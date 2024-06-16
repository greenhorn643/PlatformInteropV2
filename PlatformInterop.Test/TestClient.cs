namespace PlatformInterop.Test;

public readonly record struct MethodCodeSet
{
	public required byte AddRecord { get; init; }
	public required byte AddTwoRecords { get; init; }
	public required byte AddThreeRecords { get; init; }
	public required byte AddRecords { get; init; }
	public required byte GetRecordCount { get; init; }
	public required byte GetRecords { get; init; }
}

internal class TestClient<TRecord> : IDisposable
{
	private readonly PlatformInteropClient interopClient;
	private IDisposable? interopDisposer;
	private readonly Thread interopThread;
	private readonly MethodCodeSet methodCodeSet;
	private readonly List<TRecord> recordsLocalCopy = [];

	public IReadOnlyList<TRecord> RecordsLocalCopy => recordsLocalCopy;

	public TestClient(MethodCodeSet methodCodeSet)
	{
		(interopClient, interopDisposer) = PlatformInteropClient.Create("PlatformInterop.Test.Server.exe");
		interopClient.RegisterMethod<Unit>(methodCodeSet.AddRecord);
		interopClient.RegisterMethod<Unit>(methodCodeSet.AddTwoRecords);
		interopClient.RegisterMethod<Unit>(methodCodeSet.AddThreeRecords);
		interopClient.RegisterMethod<Unit>(methodCodeSet.AddRecords);
		interopClient.RegisterMethod<int>(methodCodeSet.GetRecordCount);
		interopClient.RegisterMethod<IEnumerable<TRecord>>(methodCodeSet.GetRecords);

		this.methodCodeSet = methodCodeSet;
		interopThread = new Thread(interopClient.Run);
		interopThread.Start();
	}

	public Task AddRecordAsync(TRecord r)
	{
		recordsLocalCopy.Add(r);

		return interopClient.CallMethodAsync(
			methodCodeSet.AddRecord,
			r);
	}

	public Task AddTwoRecordsAsync(TRecord r1, TRecord r2)
	{
		recordsLocalCopy.Add(r1);
		recordsLocalCopy.Add(r2);

		return interopClient.CallMethodAsync(
			methodCodeSet.AddTwoRecords,
			r1,
			r2);
	}

	public Task AddThreeRecordsAsync(TRecord r1, TRecord r2, TRecord r3)
	{
		recordsLocalCopy.Add(r1);
		recordsLocalCopy.Add(r2);
		recordsLocalCopy.Add(r3);

		return interopClient.CallMethodAsync(
			methodCodeSet.AddThreeRecords,
			r1,
			r2,
			r3);
	}

	public Task AddRecordsAsync(IEnumerable<TRecord> records)
	{
		recordsLocalCopy.AddRange(records);

		return interopClient.CallMethodAsync(
			methodCodeSet.AddRecords,
			records);
	}

	public Task<int> GetRecordCountAsync()
	{
		return interopClient.CallMethodAsync(
			Proxy<int>.Value,
			methodCodeSet.GetRecordCount);
	}

	public Task<IEnumerable<TRecord>> GetRecordsAsync()
	{
		return interopClient.CallMethodAsync(
			Proxy<IEnumerable<TRecord>>.Value,
			methodCodeSet.GetRecords);
	}

	public void Dispose()
	{
		if (interopDisposer != null)
		{
			interopDisposer.Dispose();
			interopDisposer = null;
			interopThread.Join();
		}
	}
}
