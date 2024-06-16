using PlatformInterop.Test.Shared;
using UnicodeRandom;

namespace PlatformInterop.Test;

[TestClass]
public class UnsolicitedTest
{
	[TestMethod]
	[DataRow(100, 10000)]
	public async Task CanReceiveUnsolicitedMessages(int nMessages, int messageLength)
	{
		var (interopClient, disposer) = PlatformInteropClient.Create("PlatformInterop.Test.Server.exe");

		List<string> receivedMessages = [];
		TaskCompletionSource allMessagesReceived = new();

		interopClient.RegisterMethod<Unit>(MethodCodes.RequestUnsolicited);
		interopClient.RegisterUnsolicited(MethodCodes.UnsolicitedMessage, (string message) =>
		{
			receivedMessages.Add(message);
			if (receivedMessages.Count == nMessages)
			{
				allMessagesReceived.SetResult();
			}
		});

		var interopThread = new Thread(interopClient.Run);
		var rng = new Random();

		{
			using var _ = disposer;
			interopThread.Start();

			var messages = Enumerable.Range(0, nMessages)
				.Select(_ => rng.NextUnicodeString(messageLength))
				.ToArray();

			await interopClient.CallMethodAsync(MethodCodes.RequestUnsolicited, messages);

			await allMessagesReceived.Task;

			Assert.AreEqual(messages.Length, receivedMessages.Count);

			for (int i = 0; i < messages.Length; i++)
			{
				Assert.AreEqual(messages[i], receivedMessages[i]);
			}
		}

		interopThread.Join();
	}
}
