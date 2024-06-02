namespace PlatformInterop.Internal.Serializer;

public class MemoryPackSerializer : ISerializer
{
	public T? Deserialize<T>(ReadOnlySpan<byte> bytes)
	{
		return MemoryPack.MemoryPackSerializer.Deserialize<T>(bytes);
	}

	public byte[] Serialize<T>(in T item)
	{
		return MemoryPack.MemoryPackSerializer.Serialize(in item);
	}
}
