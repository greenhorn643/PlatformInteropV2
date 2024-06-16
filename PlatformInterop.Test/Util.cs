using PlatformInterop.Test.Shared;

namespace PlatformInterop.Test;

internal static class Util
{
	public static TestClient<ExampleRecord> CreateRecordClient()
	{
		return new TestClient<ExampleRecord>(new()
		{
			AddRecord = MethodCodes.AddRecord,
			AddTwoRecords = MethodCodes.AddTwoRecords,
			AddThreeRecords = MethodCodes.AddThreeRecords,
			AddRecords = MethodCodes.AddRecords,
			GetRecordCount = MethodCodes.GetRecordCount,
			GetRecords = MethodCodes.GetRecords,
		});
	}

	public static TestClient<ExampleRecord> CreateAsyncRecordClient()
	{
		return new TestClient<ExampleRecord>(new()
		{
			AddRecord = MethodCodes.AddRecordAsync,
			AddTwoRecords = MethodCodes.AddTwoRecordsAsync,
			AddThreeRecords = MethodCodes.AddThreeRecordsAsync,
			AddRecords = MethodCodes.AddRecordsAsync,
			GetRecordCount = MethodCodes.GetRecordCountAsync,
			GetRecords = MethodCodes.GetRecordsAsync,
		});
	}

	public static TestClient<ExampleRecordStruct> CreateRecordStructClient()
	{
		return new TestClient<ExampleRecordStruct>(new()
		{
			AddRecord = MethodCodes.AddRecordStruct,
			AddTwoRecords = MethodCodes.AddTwoRecordStructs,
			AddThreeRecords = MethodCodes.AddThreeRecordStructs,
			AddRecords = MethodCodes.AddRecordStructs,
			GetRecordCount = MethodCodes.GetRecordStructCount,
			GetRecords = MethodCodes.GetRecordStructs,
		});
	}

	public static TestClient<ExampleRecordStruct> CreateAsyncRecordStructClient()
	{
		return new TestClient<ExampleRecordStruct>(new()
		{
			AddRecord = MethodCodes.AddRecordStructAsync,
			AddTwoRecords = MethodCodes.AddTwoRecordStructsAsync,
			AddThreeRecords = MethodCodes.AddThreeRecordStructsAsync,
			AddRecords = MethodCodes.AddRecordStructsAsync,
			GetRecordCount = MethodCodes.GetRecordStructCountAsync,
			GetRecords = MethodCodes.GetRecordStructsAsync,
		});
	}
}
