using PlatformInterop;

var (client, disposer) = PlatformInteropClient.Create("PlatformInterop.Console.Server.exe");

var myObj = new MyClass(client);

Task.Run(client.Run);

var x = await myObj.GetNumber();

Console.WriteLine(x);

var s = await myObj.AddAndConvertToString(13, 4.23);

Console.WriteLine(s);

disposer.Dispose();

class MyClass
{
	private readonly PlatformInteropClient client;

	public MyClass(PlatformInteropClient client)
	{
		this.client = client;

		client.RegisterMethod<int>(1);
		client.RegisterMethod<string>(2);
	}

	public Task<int> GetNumber()
	{
		return client.CallMethodAsync(Proxy<int>.Value, 1);
	}

	public Task<string> AddAndConvertToString(int x, double y)
	{
		return client.CallMethodAsync(Proxy<string>.Value, 2, x, y);
	}
}
