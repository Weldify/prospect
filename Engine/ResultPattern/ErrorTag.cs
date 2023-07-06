namespace Prospect.Engine;

public struct ErrorTag {}
public struct ErrorTag<TError> {
    public readonly TError Error;
	public ErrorTag( TError error ) => Error = error;
}