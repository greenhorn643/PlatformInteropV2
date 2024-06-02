namespace PlatformInterop.Internal;
public interface ISerializer
{
	byte[] Serialize<T>(in T item);
	T? Deserialize<T>(ReadOnlySpan<byte> bytes);
}

public interface IChannel
{
	ValueTask SendAsync(ReadOnlyMemory<byte> bytes);
	ValueTask<int> ReceiveAsync(Buffer buffer);
}