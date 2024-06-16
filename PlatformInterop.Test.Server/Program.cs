using PlatformInterop;
using PlatformInterop.Test.Server;
using PlatformInterop.Test.Shared;

var (server, disposer) = PlatformInteropServer.Create(args[0], args[1]);

var recordService = new TestService<ExampleRecord>();
var recordStructService = new TestService<ExampleRecordStruct>();

server.RegisterHandler(MethodCodes.AddRecord,
	(ExampleRecord r) =>
	{
		recordService.AddRecord(r);
		return Task.FromResult(Unit.Value);
	});

server.RegisterHandler(MethodCodes.AddTwoRecords,
	((ExampleRecord r1, ExampleRecord r2) args) =>
	{
		recordService.AddTwoRecords(args.r1, args.r2);
		return Task.FromResult(Unit.Value);
	});

server.RegisterHandler(MethodCodes.AddThreeRecords,
	((ExampleRecord r1, ExampleRecord r2, ExampleRecord r3) args) =>
	{
		recordService.AddThreeRecords(args.r1, args.r2, args.r3);
		return Task.FromResult(Unit.Value);
	});

server.RegisterHandler(MethodCodes.AddRecords,
	(IEnumerable<ExampleRecord> records) =>
	{
		recordService.AddRecords(records);
		return Task.FromResult(Unit.Value);
	});

server.RegisterHandler(MethodCodes.GetRecordCount,
	(Unit _) =>
	{
		return Task.FromResult(recordService.GetRecordCount());
	});

server.RegisterHandler(MethodCodes.GetRecords,
	(Unit _) =>
	{
		return Task.FromResult(recordService.GetRecords());
	});

server.RegisterHandler(MethodCodes.AddRecordAsync,
	async (ExampleRecord r) =>
	{
		await recordService.AddRecordAsync(r);
		return Unit.Value;
	});

server.RegisterHandler(MethodCodes.AddTwoRecordsAsync,
	async ((ExampleRecord r1, ExampleRecord r2) args) =>
	{
		await recordService.AddTwoRecordsAsync(args.r1, args.r2);
		return Unit.Value;
	});

server.RegisterHandler(MethodCodes.AddThreeRecordsAsync,
	async ((ExampleRecord r1, ExampleRecord r2, ExampleRecord r3) args) =>
	{
		await recordService.AddThreeRecordsAsync(args.r1, args.r2, args.r3);
		return Unit.Value;
	});

server.RegisterHandler(MethodCodes.AddRecordsAsync,
	async (IEnumerable<ExampleRecord> records) =>
	{
		await recordService.AddRecordsAsync(records);
		return Unit.Value;
	});

server.RegisterHandler(MethodCodes.GetRecordCountAsync,
	(Unit _) =>
	{
		return recordService.GetRecordCountAsync();
	});

server.RegisterHandler(MethodCodes.GetRecordsAsync,
	(Unit _) =>
	{
		return recordService.GetRecordsAsync();
	});

server.RegisterHandler(MethodCodes.AddRecordStruct,
	(ExampleRecordStruct r) =>
	{
		recordStructService.AddRecord(r);
		return Task.FromResult(Unit.Value);
	});

server.RegisterHandler(MethodCodes.AddTwoRecordStructs,
	((ExampleRecordStruct r1, ExampleRecordStruct r2) args) =>
	{
		recordStructService.AddTwoRecords(args.r1, args.r2);
		return Task.FromResult(Unit.Value);
	});

server.RegisterHandler(MethodCodes.AddThreeRecordStructs,
	((ExampleRecordStruct r1, ExampleRecordStruct r2, ExampleRecordStruct r3) args) =>
	{
		recordStructService.AddThreeRecords(args.r1, args.r2, args.r3);
		return Task.FromResult(Unit.Value);
	});

server.RegisterHandler(MethodCodes.AddRecordStructs,
	(IEnumerable<ExampleRecordStruct> records) =>
	{
		recordStructService.AddRecords(records);
		return Task.FromResult(Unit.Value);
	});

server.RegisterHandler(MethodCodes.GetRecordStructCount,
	(Unit _) =>
	{
		return Task.FromResult(recordStructService.GetRecordCount());
	});

server.RegisterHandler(MethodCodes.GetRecordStructs,
	(Unit _) =>
	{
		return Task.FromResult(recordStructService.GetRecords());
	});

server.RegisterHandler(MethodCodes.AddRecordStructAsync,
	async (ExampleRecordStruct r) =>
	{
		await recordStructService.AddRecordAsync(r);
		return Unit.Value;
	});

server.RegisterHandler(MethodCodes.AddTwoRecordStructsAsync,
	async ((ExampleRecordStruct r1, ExampleRecordStruct r2) args) =>
	{
		await recordStructService.AddTwoRecordsAsync(args.r1, args.r2);
		return Unit.Value;
	});

server.RegisterHandler(MethodCodes.AddThreeRecordStructsAsync,
	async ((ExampleRecordStruct r1, ExampleRecordStruct r2, ExampleRecordStruct r3) args) =>
	{
		await recordStructService.AddThreeRecordsAsync(args.r1, args.r2, args.r3);
		return Unit.Value;
	});

server.RegisterHandler(MethodCodes.AddRecordStructsAsync,
	async (IEnumerable<ExampleRecordStruct> records) =>
	{
		await recordStructService.AddRecordsAsync(records);
		return Unit.Value;
	});

server.RegisterHandler(MethodCodes.GetRecordStructCountAsync,
	(Unit _) =>
	{
		return recordStructService.GetRecordCountAsync();
	});

server.RegisterHandler(MethodCodes.GetRecordStructsAsync,
	(Unit _) =>
	{
		return recordStructService.GetRecordsAsync();
	});

server.RegisterHandler(MethodCodes.Is64BitProcess,
	(Unit _) => Task.FromResult(Environment.Is64BitProcess));

server.RegisterHandler(MethodCodes.RequestUnsolicited,
	(IEnumerable<string> messages) =>
	{
		foreach (var message in messages)
		{
			server.SendUnsolicited(MethodCodes.UnsolicitedMessage, message);
		}
		return Task.FromResult(Unit.Value);
	});

server.Run();

disposer.Dispose();