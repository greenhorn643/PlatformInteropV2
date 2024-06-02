namespace PlatformInterop.Internal;

public struct RequestHeader
{
	public byte MethodCode { get; set; }
	public Guid CallerId { get; set; }
	public int BodyLength { get; set; }
}

public class Request<TBody>
{
	public RequestHeader Header { get; set; }
	public required TBody Body { get; set; }
}

public static class Request
{
	public static Request<TBody> New<TBody>(byte methodCode, TBody body)
	{
		return new Request<TBody>
		{
			Header = new RequestHeader
			{
				MethodCode = methodCode,
				CallerId = Guid.NewGuid(),
			},
			Body = body,
		};
	}
}
