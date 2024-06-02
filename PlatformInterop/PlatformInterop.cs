using PlatformInterop.Internal.Channel;
using PlatformInterop.Internal.Serializer;
using System.Diagnostics;
using System.IO.Pipes;

namespace PlatformInterop;
public class PlatformInteropClient(ClientPipeChannel channel, MemoryPackSerializer serializer)
	: Internal.PlatformInteropClient<ClientPipeChannel, MemoryPackSerializer>(channel, serializer)
{
	public static (PlatformInteropClient, IDisposable) Create(string serverExecutablePath)
	{
		var inputStream = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
		var outputStream = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);

		var channel = new ClientPipeChannel(inputStream, outputStream);
		var serializer = new MemoryPackSerializer();

		var serverStartArgs = outputStream.GetClientHandleAsString() + " " + inputStream.GetClientHandleAsString();

		var process = new Process();
		process.StartInfo.FileName = serverExecutablePath;
		process.StartInfo.Arguments = serverStartArgs;
		process.Start();

		var client = new PlatformInteropClient(channel, serializer);
		var disposable = new Disposer(inputStream, outputStream);

		return (client, disposable);
	}
}

public class PlatformInteropServer(ServerPipeChannel channel, MemoryPackSerializer serializer)
	: Internal.PlatformInteropServer<ServerPipeChannel, MemoryPackSerializer>(channel, serializer)
{
	public static (PlatformInteropServer, IDisposable) Create(string inputPipeHandle, string outputPipeHandle)
	{
		var inputStream = new AnonymousPipeClientStream(PipeDirection.In, inputPipeHandle);
		var outputStream = new AnonymousPipeClientStream(PipeDirection.Out, outputPipeHandle);

		var channel = new ServerPipeChannel(inputStream, outputStream);
		var serializer = new MemoryPackSerializer();

		var server = new PlatformInteropServer(channel, serializer);
		var disposable = new Disposer(inputStream, outputStream);

		return (server, disposable);
	}
}