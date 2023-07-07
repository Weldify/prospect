namespace Prospect.Engine;

public struct OkTag { }
public readonly struct OkTag<TValue> {
	public readonly TValue Value;
	public OkTag( TValue value ) => Value = value;
}