using System;
using System.Collections.Concurrent;
using System.Reflection.PortableExecutable;
using System.Text.Json;

namespace PlatformInterop.Internal;

public partial class PlatformInteropClient<TChannel, TSerializer>
{
	private readonly byte[] responseHeaderBuffer = serializer.Serialize(new ResponseHeader());
	private byte[] responseBodyBuffer = [];
	private readonly int responseHeaderLength = serializer.Serialize(new ResponseHeader()).Length;

	private readonly Action<ResponseHeader, Buffer>?[] dispatchByMethodCode = new Action<ResponseHeader, Buffer>[256];
	private readonly ConcurrentDictionary<Guid, object> pendingMethodCalls = [];

	private readonly BlockingCollection<Action> senderQueue = [];
	private readonly BlockingCollection<Action> unsolicitedQueue = [];

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
#if DEBUG
				Console.WriteLine($"{nameof(PlatformInteropClient)} received response header {JsonSerializer.Serialize(currentHeader)} ({responseHeaderBuffer.Length} bytes)");
#endif
				hasHeader = true;
			}
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

	private void UnsolicitedLoop()
	{
		while (true)
		{
			var action = unsolicitedQueue.Take();
			try
			{
				action();
			}
			catch (Exception e)
			{
				Console.WriteLine($"warning: an unsolicited message handler threw an exception: {e}");
			}
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
#if DEBUG
			Console.WriteLine($"{nameof(PlatformInteropClient)} sending request header {JsonSerializer.Serialize(headerWithBodyLength)} ({header.Length} bytes)");
#endif
			channel.Send(header);
#if DEBUG
			Console.WriteLine($"{nameof(PlatformInteropClient)} sending request body ({body.Length} bytes)");
#endif
			channel.Send(body);
#if DEBUG
			Console.WriteLine($"{nameof(PlatformInteropClient)} completed request packet send");
#endif
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

#if DEBUG
			Console.WriteLine($"{nameof(PlatformInteropClient)} received response body of type {typeof(TReturnType).Name} ({responseHeaderBuffer.Length} bytes)");
#endif

			if (pendingMethodCalls.TryRemove(header.CallerId, out var t))
			{
				var tcs = (TaskCompletionSource<TReturnType>)t;
				Task.Run(() =>
				{
#if DEBUG
					Console.WriteLine($"{nameof(PlatformInteropClient)} returning result {value}");
#endif
					tcs.SetResult(value);
				});
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
				Task.Run(() =>
				{
#if DEBUG
					Console.WriteLine($"{nameof(PlatformInteropClient)} returning exception {error}");
#endif
					tcs.SetException(error);
				});
			}
		}
	}

	private Action<ResponseHeader, Buffer> DeserializeResponseBodyAndPostUnsolicited<TReturnType>(Action<TReturnType> handler)
	{
		void action(ResponseHeader header, Buffer buffer)
		{
			if (responseBodyBuffer.Length < header.BodyLength)
			{
				responseBodyBuffer = new byte[header.BodyLength];
			}

			if (!header.IsSuccess)
			{
				throw new PlatformInteropClientException("invalid header: exceptions are not supported for unsolicited responses");
			}

			var bodySpan = responseBodyBuffer.AsSpan()[..header.BodyLength];
			buffer.PopRange(bodySpan);
			var value = serializer.Deserialize<TReturnType>(bodySpan)
				?? throw new PlatformInteropClientException($"failed to deserialize return type {typeof(TReturnType).Name}");
#if DEBUG
			Console.WriteLine($"{nameof(PlatformInteropClient)} received response body of type {typeof(TReturnType).Name} ({responseHeaderBuffer.Length} bytes)");
#endif
			unsolicitedQueue.Add(() => handler(value));
		}

		return action;
	}
}
