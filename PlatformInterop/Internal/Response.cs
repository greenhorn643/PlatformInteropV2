namespace PlatformInterop.Internal;
public struct ResponseHeader
{
	public byte MethodCode { get; set; }
	public Guid CallerId { get; set; }
	public bool IsSuccess { get; set; }
	public int BodyLength { get; set; }
}

public class Response<TValue>
{
	public ResponseHeader Header { get; set; }
	public TValue? Value { get; set; }
	public Exception? Error { get; set; }
}