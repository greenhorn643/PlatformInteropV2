using PlatformInterop;
using PlatformInterop.Server;

var (server, disposer) = PlatformInteropServer.Create(args[0], args[1]);

var service = new PersonService();

server.RegisterHandler(1, (Unit _) => service.IsServiceRunningAs64BitProcess());
server.RegisterHandler(2, (Person person) => service.AddPersonAsync(person));
server.RegisterHandler(3, async (int id) => { await service.RemovePersonAsync(id); return Unit.Value; });
server.RegisterHandler(4, (int id) => service.GetPersonAsync(id));
server.RegisterHandler(5, (Unit _) => service.GetPeopleAsync());

server.Run();

disposer.Dispose();