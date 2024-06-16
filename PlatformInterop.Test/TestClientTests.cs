using PlatformInterop.Test.Shared;

namespace PlatformInterop.Test;

[TestClass]
public class TestClientTests
{
	private readonly Random rng = new();

	[TestMethod]
	public async Task CanCallOneArgumentMethod()
	{
		await CanCallOneArgumentMethodImpl(Util.CreateRecordClient);
		await CanCallOneArgumentMethodImpl(Util.CreateAsyncRecordClient);
		await CanCallOneArgumentMethodImpl(Util.CreateRecordStructClient);
		await CanCallOneArgumentMethodImpl(Util.CreateAsyncRecordStructClient);
	}

	[TestMethod]
	public async Task CanCallTwoArgumentMethod()
	{
		await CanCallTwoArgumentMethodImpl(Util.CreateRecordClient);
		await CanCallTwoArgumentMethodImpl(Util.CreateAsyncRecordClient);
		await CanCallTwoArgumentMethodImpl(Util.CreateRecordStructClient);
		await CanCallTwoArgumentMethodImpl(Util.CreateAsyncRecordStructClient);
	}

	[TestMethod]
	public async Task CanCallThreeArgumentMethod()
	{
		await CanCallThreeArgumentMethodImpl(Util.CreateRecordClient);
		await CanCallThreeArgumentMethodImpl(Util.CreateAsyncRecordClient);
		await CanCallThreeArgumentMethodImpl(Util.CreateRecordStructClient);
		await CanCallThreeArgumentMethodImpl(Util.CreateAsyncRecordStructClient);
	}

	private async Task CanCallOneArgumentMethodImpl<TRecord>(Func<TestClient<TRecord>> clientFactory)
		where TRecord : IRandom<TRecord>
	{
		using var client = clientFactory();

		var r = TRecord.Random(rng);

		await client.AddRecordAsync(r);

		var rs = (await client.GetRecordsAsync()).ToArray();

		Assert.IsNotNull(rs);
		Assert.AreEqual(1, rs.Length);
		Assert.AreEqual(r, rs[0]);
	}


	private async Task CanCallTwoArgumentMethodImpl<TRecord>(Func<TestClient<TRecord>> clientFactory)
		where TRecord : IRandom<TRecord>
	{
		using var client = clientFactory();

		var r1 = TRecord.Random(rng);
		var r2 = TRecord.Random(rng);

		await client.AddTwoRecordsAsync(r1, r2);

		var rs = (await client.GetRecordsAsync()).ToArray();

		Assert.IsNotNull(rs);
		Assert.AreEqual(2, rs.Length);
		Assert.AreEqual(r1, rs[0]);
		Assert.AreEqual(r2, rs[1]);
	}


	private async Task CanCallThreeArgumentMethodImpl<TRecord>(Func<TestClient<TRecord>> clientFactory)
		where TRecord : IRandom<TRecord>
	{
		using var client = clientFactory();

		var r1 = TRecord.Random(rng);
		var r2 = TRecord.Random(rng);
		var r3 = TRecord.Random(rng);

		await client.AddThreeRecordsAsync(r1, r2, r3);

		var rs = (await client.GetRecordsAsync()).ToArray();

		Assert.IsNotNull(rs);
		Assert.AreEqual(3, rs.Length);
		Assert.AreEqual(r1, rs[0]);
		Assert.AreEqual(r2, rs[1]);
		Assert.AreEqual(r3, rs[2]);
	}
}
