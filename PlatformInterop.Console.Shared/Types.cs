using MemoryPack;

namespace PlatformInterop.Shared;

[MemoryPackable]
public partial record Address(string Street, int Number, int? Postcode);

[MemoryPackable]
public partial record Person(string FirstName, string LastName, int Age, Address? Address);