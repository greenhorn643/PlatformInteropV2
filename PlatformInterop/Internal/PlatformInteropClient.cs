namespace PlatformInterop.Internal;

public partial class PlatformInteropClient<TChannel, TSerializer>(
	TChannel channel,
	TSerializer serializer)
	where TChannel : IChannel
	where TSerializer : ISerializer
{
	public void Run()
	{
		new Thread(SenderLoop)
		{
			IsBackground = true
		}.Start();

		new Thread(UnsolicitedLoop)
		{
			IsBackground = true
		}.Start();

		ReceiveLoop();
	}

	public void RegisterMethod<TReturnType>(byte methodCode)
	{
		if (dispatchByMethodCode[methodCode] != null)
		{
			throw new PlatformInteropClientException($"a method with code {methodCode} has already been registered");
		}

		dispatchByMethodCode[methodCode] = DeserializeResponseBodyAndPostResponse<TReturnType>;
	}

	public void RegisterUnsolicited<TValue>(byte methodCode, Action<TValue> handler)
	{
		if (dispatchByMethodCode[methodCode] != null)
		{
			throw new PlatformInteropClientException($"a method with code {methodCode} has already been registered");
		}

		dispatchByMethodCode[methodCode] = DeserializeResponseBodyAndPostUnsolicited(handler);
	}

	public Task<TR> CallMethodAsync<TR>(Proxy<TR> _, byte methodCode)
	{
		return CallMethodImplAsync(Proxy<TR>.Value, Request.New(methodCode, Unit.Value));
	}

	public Task CallMethodAsync(byte methodCode)
	{
		return CallMethodImplAsync(Proxy<Unit>.Value, Request.New(methodCode, Unit.Value));
	}

	public Task<TR> CallMethodAsync<TR, T0>(Proxy<TR> _, byte methodCode, T0 arg0)
	{
		return CallMethodImplAsync(Proxy<TR>.Value, Request.New(methodCode, arg0));
	}

	public Task CallMethodAsync<T0>(byte methodCode, T0 arg0)
	{
		return CallMethodImplAsync(Proxy<Unit>.Value, Request.New(methodCode, arg0));
	}

	public Task<TR> CallMethodAsync<TR, T0, T1>(Proxy<TR> _, byte methodCode, T0 arg0, T1 arg1)
	{
		return CallMethodImplAsync(Proxy<TR>.Value, Request.New(methodCode, (arg0, arg1)));
	}

	public Task CallMethodAsync<T0, T1>(byte methodCode, T0 arg0, T1 arg1)
	{
		return CallMethodImplAsync(Proxy<Unit>.Value, Request.New(methodCode, (arg0, arg1)));
	}

	public Task<TR> CallMethodAsync<TR, T0, T1, T2>(Proxy<TR> _, byte methodCode, T0 arg0, T1 arg1, T2 arg2)
	{
		return CallMethodImplAsync(Proxy<TR>.Value, Request.New(methodCode, (arg0, arg1, arg2)));
	}

	public Task CallMethodAsync<T0, T1, T2>(byte methodCode, T0 arg0, T1 arg1, T2 arg2)
	{
		return CallMethodImplAsync(Proxy<Unit>.Value, Request.New(methodCode, (arg0, arg1, arg2)));
	}

	public Task<TR> CallMethodAsync<TR, T0, T1, T2, T3>(Proxy<TR> _, byte methodCode, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
	{
		return CallMethodImplAsync(Proxy<TR>.Value, Request.New(methodCode, (arg0, arg1, arg2, arg3)));
	}

	public Task CallMethodAsync<T0, T1, T2, T3>(byte methodCode, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
	{
		return CallMethodImplAsync(Proxy<Unit>.Value, Request.New(methodCode, (arg0, arg1, arg2, arg3)));
	}

	public Task<TR> CallMethodAsync<TR, T0, T1, T2, T3, T4>(Proxy<TR> _, byte methodCode, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		return CallMethodImplAsync(Proxy<TR>.Value, Request.New(methodCode, (arg0, arg1, arg2, arg3, arg4)));
	}

	public Task CallMethodAsync<T0, T1, T2, T3, T4>(byte methodCode, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		return CallMethodImplAsync(Proxy<Unit>.Value, Request.New(methodCode, (arg0, arg1, arg2, arg3, arg4)));
	}

	public Task<TR> CallMethodAsync<TR, T0, T1, T2, T3, T4, T5>(Proxy<TR> _, byte methodCode, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		return CallMethodImplAsync(Proxy<TR>.Value, Request.New(methodCode, (arg0, arg1, arg2, arg3, arg4, arg5)));
	}

	public Task CallMethodAsync<T0, T1, T2, T3, T4, T5>(byte methodCode, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		return CallMethodImplAsync(Proxy<Unit>.Value, Request.New(methodCode, (arg0, arg1, arg2, arg3, arg4, arg5)));
	}
}
