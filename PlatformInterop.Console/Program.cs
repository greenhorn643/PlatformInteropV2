using PlatformInterop;
using PlatformInterop.Client;
using PlatformInterop.Shared;

var (client, disposer) = PlatformInteropClient.Create("PlatformInterop.Console.Server.exe");

var personClient = new PersonClient(client);

new Thread(client.Run)
{
	IsBackground = true
}.Start();

int nItems = 1000000;

var t0 = DateTime.Now;

var tasks = Enumerable.Range(0, nItems).Select(_ => personClient.AddPersonAsync(new Person("alex", "palmer", 33, null)));

await Task.WhenAll(tasks);

/*for (int i = 0; i < nItems; i++)
{
	await personClient.AddPersonAsync(new Person("alex", "palmer", 33, null));
}
*/
var t1 = DateTime.Now;

var elapsed = t1 - t0;

Console.WriteLine($"wrote {nItems} Person objects in {elapsed.TotalSeconds} seconds");

t0 = DateTime.Now;

var people = await personClient.GetPeopleAsync();

t1 = DateTime.Now;

elapsed = t1 - t0;

Console.WriteLine($"read {people.Count} Person objects in {elapsed.TotalSeconds} seconds");

foreach (var person in people)
{
	if (person != new Person("alex", "palmer", 33, null))
	{
		throw new Exception("people not equal");
	}
}

disposer.Dispose();