namespace PlatformInterop.Internal;

public partial class PlatformInteropServer<TChannel, TSerializer>(
	TChannel channel,
	TSerializer serializer)
	where TChannel : IChannel
	where TSerializer : ISerializer
{
	public void Run()
	{
		new Thread(ResponderLoop)
		{
			IsBackground = true
		}.Start();

		new Thread(HandlerLoop)
		{
			IsBackground = true
		}.Start();

		ReceiveLoop();
	}

	public void RegisterHandler<RT, TArgs>(byte methodCode, Func<TArgs, Task<RT>> handler)
	{
		if (requestHandlers[methodCode] != null)
		{
			throw new PlatformInteropServerException($"a handler has already been registered for method code {methodCode}");
		}

		requestHandlers[methodCode] = handler;
		dispatchByMethodCode[methodCode] = CallMethodAndRespond<RT, TArgs>;
	}

	public void SendUnsolicited<TValue>(byte methodCode, TValue value)
	{
		byte[] body = serializer.Serialize(value);

		var responseHeader = new ResponseHeader
		{
			MethodCode = methodCode,
			CallerId = Guid.Empty,
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
}