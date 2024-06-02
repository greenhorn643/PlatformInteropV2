using System.Collections.Concurrent;

namespace PlatformInterop.Internal;

public partial class PlatformInteropClient<TChannel, TSerializer>
{
	private readonly byte[] responseHeaderBuffer = serializer.Serialize(new ResponseHeader());
	private byte[] responseBodyBuffer = [];
	private readonly int responseHeaderLength = serializer.Serialize(new ResponseHeader()).Length;

	private readonly Action<ResponseHeader, Buffer>?[] dispatchByMethodCode = new Action<ResponseHeader, Buffer>[256];
	private readonly ConcurrentDictionary<Guid, object> pendingMethodCalls = [];

	private readonly BlockingCollection<Action> responderQueue = [];
	private readonly BlockingCollection<Action> senderQueue = [];

	private void ReceiveLoop()
	{
		Buffer buffer = [];
		ResponseHeader currentHeader = default;
		bool hasHeader = false;

		while (true)
		{
			if (hasHeader)
			{
				if (buffer.Count < currentHeader.BodyLength)
				{
					if (channel.Receive(buffer) == 0)
					{
						break;
					}
					continue;
				}

				if (dispatchByMethodCode[currentHeader.MethodCode] == null)
				{
					throw new PlatformInteropClientException($"no method registered with method code {currentHeader.MethodCode}");
				}

				dispatchByMethodCode[currentHeader.MethodCode]!(currentHeader, buffer);
				hasHeader = false;
			}
			else
			{
				if (buffer.Count < responseHeaderLength)
				{
					if (channel.Receive(buffer) == 0)
					{
						break;
					}
					continue;
				}

				buffer.PopRange(responseHeaderBuffer);
				currentHeader = serializer.Deserialize<ResponseHeader>(responseHeaderBuffer);
				hasHeader = true;
			}
		}
	}

	private void ResponderLoop()
	{
		while (true)
		{
			var action = responderQueue.Take();
			action();
		}
	}

	private void SenderLoop()
	{
		while (true)
		{
			var action = senderQueue.Take();
			action();
		}
	}

	private Task<TR> CallMethodImplAsync<TR, TBody>(Proxy<TR> _, Request<TBody> req)
	{
		byte[] body = serializer.Serialize(req.Body);

		var headerWithBodyLength = req.Header;
		headerWithBodyLength.BodyLength = body.Length;

		var tcs = new TaskCompletionSource<TR>();

		if (!pendingMethodCalls.TryAdd(req.Header.CallerId, tcs))
		{
			throw new PlatformInteropClientException("duplicate caller id");
		}

		byte[] header = serializer.Serialize(headerWithBodyLength);

		senderQueue.Add(() =>
		{
			channel.Send(header);
			channel.Send(body);
		});

		return tcs.Task;
	}

	private void DeserializeResponseBodyAndPostResponse<TReturnType>(ResponseHeader header, Buffer buffer)
	{
		if (responseBodyBuffer.Length < header.BodyLength)
		{
			responseBodyBuffer = new byte[header.BodyLength];
		}

		if (header.IsSuccess)
		{
			var bodySpan = responseBodyBuffer.AsSpan()[..header.BodyLength];
			buffer.PopRange(bodySpan);
			var value = serializer.Deserialize<TReturnType>(bodySpan)
				?? throw new PlatformInteropClientException($"failed to deserialize return type {typeof(TReturnType).Name}");

			if (pendingMethodCalls.TryRemove(header.CallerId, out var t))
			{
				var tcs = (TaskCompletionSource<TReturnType>)t;
				responderQueue.Add(() => tcs.SetResult(value));
			}
		}
		else
		{
			var bodySpan = responseBodyBuffer.AsSpan()[..header.BodyLength];
			buffer.PopRange(bodySpan);
			var error = serializer.Deserialize<Exception>(bodySpan)
				?? throw new PlatformInteropClientException($"failed to deserialize {typeof(Exception).Name}");

			if (pendingMethodCalls.TryRemove(header.CallerId, out var t))
			{
				var tcs = (TaskCompletionSource<TReturnType>)t;
				responderQueue.Add(() => tcs.SetException(error));
			}
		}
	}
}
