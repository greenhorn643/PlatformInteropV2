using MemoryPack;
using UnicodeRandom;

namespace PlatformInterop.Test.Shared;

public interface IRandom<TItem>
{
	static abstract TItem Random(Random rng);
}

[MemoryPackable]
public partial record ExampleRecord : IRandom<ExampleRecord>
{
	public required int Id { get; init; }
	public required string Name { get; init; }
	public required decimal Price { get; init; }

	public static ExampleRecord Random(Random rng)
	{
		return new ExampleRecord
		{
			Id = rng.Next(int.MinValue, int.MaxValue),
			Name = rng.NextUnicodeString(0, 10),
			Price = (decimal)rng.NextDouble(),
		};
	}
}

[MemoryPackable]
public readonly partial record struct ExampleRecordStruct : IRandom<ExampleRecordStruct>
{
	public required int Id { get; init; }
	public required string Name { get; init; }
	public required decimal Price { get; init; }

	public static ExampleRecordStruct Random(Random rng)
	{
		return new ExampleRecordStruct
		{
			Id = rng.Next(int.MinValue, int.MaxValue),
			Name = rng.NextUnicodeString(0, 10),
			Price = (decimal)rng.NextDouble(),
		};
	}
}