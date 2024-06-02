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
}