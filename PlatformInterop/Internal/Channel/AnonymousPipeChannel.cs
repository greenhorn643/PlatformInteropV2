using System.IO.Pipes;

namespace PlatformInterop.Internal.Channel;

public class ClientPipeChannel(AnonymousPipeServerStream inputStream, AnonymousPipeServerStream outputStream) : IChannel
{
	public int Receive(Buffer buffer)
	{
		int nRead = inputStream.Read(buffer.GetSpan());
		buffer.Advance(nRead);
		return nRead;
	}

	public async ValueTask<int> ReceiveAsync(Buffer buffer)
	{
		int nRead = await inputStream.ReadAsync(buffer.GetMemory());
		buffer.Advance(nRead);
		return nRead;
	}

	public void Send(ReadOnlySpan<byte> bytes)
	{
		outputStream.Write(bytes);
	}

	public ValueTask SendAsync(ReadOnlyMemory<byte> bytes)
	{
		return outputStream.WriteAsync(bytes);
	}
}

public class ServerPipeChannel(AnonymousPipeClientStream inputStream, AnonymousPipeClientStream outputStream) : IChannel
{
	public int Receive(Buffer buffer)
	{
		int nRead = inputStream.Read(buffer.GetSpan());
		buffer.Advance(nRead);
		return nRead;
	}

	public async ValueTask<int> ReceiveAsync(Buffer buffer)
	{
		int nRead = await inputStream.ReadAsync(buffer.GetMemory());
		buffer.Advance(nRead);
		return nRead;
	}

	public void Send(ReadOnlySpan<byte> bytes)
	{
		outputStream.Write(bytes);
	}

	public ValueTask SendAsync(ReadOnlyMemory<byte> bytes)
	{
		return outputStream.WriteAsync(bytes);
	}
}