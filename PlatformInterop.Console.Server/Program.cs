using PlatformInterop;

var (server, disposer) = PlatformInteropServer.Create(args[0], args[1]);

var myObj = new MyClass();

server.RegisterHandler(1, (Unit _) => Task.FromResult(myObj.GetNumber()));
server.RegisterHandler(2, ((int x, double y) args) => myObj.AddAndConvertToString(args.x, args.y));

await server.Run();

disposer.Dispose();

class MyClass
{
	public int GetNumber()
	{
		return 42;
	}

	public async Task<string> AddAndConvertToString(int x, double y)
	{
		await Task.Delay(500);

		return (x + y).ToString();
	}
}