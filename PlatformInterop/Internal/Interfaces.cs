namespace PlatformInterop.Internal;
public interface ISerializer
{
	byte[] Serialize<T>(in T item);
	T? Deserialize<T>(ReadOnlySpan<byte> bytes);
}

public interface IChannel
{
	void Send(ReadOnlySpan<byte> bytes);
	ValueTask SendAsync(ReadOnlyMemory<byte> bytes);
	int Receive(Buffer buffer);
	ValueTask<int> ReceiveAsync(Buffer buffer);
}