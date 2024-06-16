using PlatformInterop.Test.Shared;

namespace PlatformInterop.Test;

[TestClass]
public class CrossPlatform
{
	[TestMethod]
	public async Task IsRunningCrossPlatform()
	{
		var (interopClient, disposer) = PlatformInteropClient.Create("PlatformInterop.Test.Server.exe");

		interopClient.RegisterMethod<bool>(MethodCodes.Is64BitProcess);

		var interopThread = new Thread(interopClient.Run);

		{
			using var _ = disposer;
			interopThread.Start();

			var serverIs64Bit = await interopClient.CallMethodAsync(Proxy<bool>.Value, MethodCodes.Is64BitProcess);

			var clientIs64Bit = Environment.Is64BitProcess;

			Assert.IsFalse(serverIs64Bit);
			Assert.IsTrue(clientIs64Bit);
		}

		interopThread.Join();
	}
}