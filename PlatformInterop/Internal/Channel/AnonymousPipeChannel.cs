using System.IO.Pipes;

namespace PlatformInterop.Internal.Channel;

public class ClientPipeChannel(AnonymousPipeServerStream inputStream, AnonymousPipeServerStream outputStream) : IChannel
{
	public async ValueTask<int> ReceiveAsync(Buffer buffer)
	{
		int nRead = await inputStream.ReadAsync(buffer.GetMemory());
		buffer.Advance(nRead);
		return nRead;
	}

	public ValueTask SendAsync(ReadOnlyMemory<byte> bytes)
	{
		return outputStream.WriteAsync(bytes);
	}
}

public class ServerPipeChannel(AnonymousPipeClientStream inputStream, AnonymousPipeClientStream outputStream) : IChannel
{

	public async ValueTask<int> ReceiveAsync(Buffer buffer)
	{
		int nRead = await inputStream.ReadAsync(buffer.GetMemory());
		buffer.Advance(nRead);
		return nRead;
	}

	public ValueTask SendAsync(ReadOnlyMemory<byte> bytes)
	{
		return outputStream.WriteAsync(bytes);
	}
}