namespace PlatformInterop.Test.Server;

internal class TestService<TRecord>
{
	private List<TRecord> records = [];

	public void AddRecord(TRecord record)
	{
		records.Add(record);
	}

	public Task AddRecordAsync(TRecord record)
	{
		return Task.Run(() => AddRecord(record));
	}

	public void AddTwoRecords(TRecord r1, TRecord r2)
	{
		records.Add(r1);
		records.Add(r2);
	}

	public Task AddTwoRecordsAsync(TRecord r1, TRecord r2)
	{
		return Task.Run(() => AddTwoRecords(r1, r2));
	}

	public void AddThreeRecords(TRecord r1, TRecord r2, TRecord r3)
	{
		records.Add(r1);
		records.Add(r2);
		records.Add(r3);
	}

	public Task AddThreeRecordsAsync(TRecord r1, TRecord r2, TRecord r3)
	{
		return Task.Run(() => AddThreeRecords(r1, r2, r3));
	}

	public void AddRecords(IEnumerable<TRecord> records)
	{
		this.records.AddRange(records);
	}

	public Task AddRecordsAsync(IEnumerable<TRecord> records)
	{
		return Task.Run(() => AddRecords(records));
	}

	public int GetRecordCount()
	{
		return records.Count;
	}

	public Task<int> GetRecordCountAsync()
	{
		return Task.Run(GetRecordCount);
	}

	public List<TRecord> GetRecords()
	{
		return [.. records];
	}

	public Task<List<TRecord>> GetRecordsAsync()
	{
		return Task.Run(GetRecords);
	}
}
