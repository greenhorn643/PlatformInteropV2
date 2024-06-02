using PlatformInterop.Server;

namespace PlatformInterop.Client;

internal class PersonClient
{
	private readonly PlatformInteropClient client;
	public PersonClient(PlatformInteropClient client)
	{
		this.client = client;

		client.RegisterMethod<bool>(1);
		client.RegisterMethod<int>(2);
		client.RegisterMethod<Unit>(3);
		client.RegisterMethod<Person?>(4);
		client.RegisterMethod<List<Person>>(5);
	}

	public Task<bool> IsServiceRunningAs64BitProcess()
	{
		return client.CallMethodAsync(Proxy<bool>.Value, 1);
	}

	public Task<int> AddPersonAsync(Person person)
	{
		return client.CallMethodAsync(Proxy<int>.Value, 2, person);
	}

	public Task RemovePersonAsync(int id)
	{
		return client.CallMethodAsync(3, id);
	}

	public Task<Person?> GetPersonAsync(int id)
	{
		return client.CallMethodAsync(Proxy<Person?>.Value, 4, id);
	}

	public Task<List<Person>> GetPeopleAsync()
	{
		return client.CallMethodAsync(Proxy<List<Person>>.Value, 5);
	}
}