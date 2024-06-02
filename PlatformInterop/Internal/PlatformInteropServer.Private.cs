using System.Collections.Concurrent;

namespace PlatformInterop.Internal;

public partial class PlatformInteropServer<TChannel, TSerializer>
{
	private readonly byte[] requestHeaderBuffer = serializer.Serialize(new RequestHeader());
	private readonly int requestHeaderLength = serializer.Serialize(new RequestHeader()).Length;
	private byte[] requestBodyBuffer = [];
	private readonly object?[] requestHandlers = new object[256];
	private readonly Action<RequestHeader, Buffer>?[] dispatchByMethodCode = new Action<RequestHeader, Buffer>[256];

	private readonly BlockingCollection<Action> responderQueue = [];

	private void CallMethodAndRespond<RT, TArgs>(RequestHeader header, Buffer buffer)
	{
		if (requestBodyBuffer.Length < header.BodyLength)
		{
			requestBodyBuffer = new byte[header.BodyLength];
		}

		var bodySpan = requestBodyBuffer.AsSpan()[..header.BodyLength];
		buffer.PopRange(bodySpan);
		var args = serializer.Deserialize<TArgs>(requestBodyBuffer)
			?? throw new PlatformInteropServerException($"failed to deserialize arguments of type {typeof(TArgs).Name}");

		Task.Run(async () =>
		{
			var handler = requestHandlers[header.MethodCode]
				?? throw new PlatformInteropServerException($"no handler registered for method code {header.MethodCode}");

			if (handler is Func<TArgs, Task<RT>> typedHandler)
			{
				try
				{
					var value = await typedHandler(args);

					var body = serializer.Serialize(value);

					var responseHeader = new ResponseHeader
					{
						MethodCode = header.MethodCode,
						CallerId = header.CallerId,
						IsSuccess = true,
						BodyLength = body.Length,
					};

					byte[] headerBytes = serializer.Serialize(responseHeader);

					responderQueue.Add(() =>
					{
						channel.Send(headerBytes);
						channel.Send(body);
					});
				}
				catch (Exception ex)
				{
					var body = serializer.Serialize(ex);

					var responseHeader = new ResponseHeader
					{
						MethodCode = header.MethodCode,
						CallerId = header.CallerId,
						IsSuccess = false,
						BodyLength = body.Length,
					};

					byte[] headerBytes = serializer.Serialize(responseHeader);

					byte[] packet = [.. headerBytes, .. body];

					responderQueue.Add(() =>
					{
						channel.Send(headerBytes);
						channel.Send(body);
					});
				}
			}
			else
			{
				throw new PlatformInteropServerException($"handler for method {header.MethodCode} is of an invalid type");
			}
		});
	}

	private void ReceiveLoop()
	{
		Buffer buffer = [];
		RequestHeader currentHeader = default;
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
					throw new PlatformInteropServerException($"no handler registered with method code {currentHeader.MethodCode}");
				}

				dispatchByMethodCode[currentHeader.MethodCode]!(currentHeader, buffer);
				hasHeader = false;
			}
			else
			{
				if (buffer.Count < requestHeaderLength)
				{
					if (channel.Receive(buffer) == 0)
					{
						break;
					}
					continue;
				}

				buffer.PopRange(requestHeaderBuffer);
				currentHeader = serializer.Deserialize<RequestHeader>(requestHeaderBuffer);
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
}