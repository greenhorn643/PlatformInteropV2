namespace PlatformInterop;

public class PlatformInteropClientException(string message) : Exception(message) { }
public class PlatformInteropServerException(string message) : Exception(message) { }

public readonly struct Unit
{
	public static readonly Unit Value = new();
}

public readonly struct Proxy<T>
{
	public static readonly Proxy<T> Value = new();
}

internal class Disposer(params IDisposable[] disposables) : IDisposable
{
	public void Dispose()
	{
		foreach (var disposable in disposables)
		{
			disposable.Dispose();
		}
		disposables = [];
	}
}