namespace Prospect.Engine;

public static class Result {
	public static OkTag Ok() => new();
	public static OkTag<T> Ok<T>( T value ) => new( value );

    public static ErrorTag Fail() => new();
	public static ErrorTag<T> Fail<T>( T error ) => new( error );
}

public struct Result<TValue> {
	static Result<TValue> _errorResult = new( false );

	public TValue Value => IsOk ? _value : throw new Exception( "Unwrapped error result!" );
	public readonly bool IsOk;
	public bool IsError => !IsOk;

	readonly TValue _value;

#nullable disable
	private Result( bool isOk, TValue value = default ) {
#nullable enable
		IsOk = isOk;
		_value = value;
	}

	public static implicit operator Result<TValue>( TValue value ) => new( true, value );
	public static implicit operator Result<TValue>( OkTag<TValue> tag ) => new( true, tag.Value );
	public static implicit operator Result<TValue>( ErrorTag tag ) => _errorResult;
}

public struct Result<TValue, TError> {
	static Result<TValue, TError> _errorResult = new( false );

	public TValue Value => IsOk ? _value : throw new Exception( "Unwrapped error result!" );
	public TError Error => IsError ? _error : throw new Exception( "Unwrapped ok result!" );
	public readonly bool IsOk;
	public bool IsError => !IsOk;

	readonly TValue _value;
	readonly TError _error;

#nullable disable
	private Result( bool isOk, TValue value = default, TError error = default ) {
#nullable enable
		IsOk = isOk;
		_value = value;
		_error = error;
	}

	public static implicit operator Result<TValue, TError>( TValue value ) => new( true, value );
	public static implicit operator Result<TValue, TError>( TError error ) => new( true, error: error );
	public static implicit operator Result<TValue, TError>( OkTag<TValue> tag ) => new( true, tag.Value );
	public static implicit operator Result<TValue, TError>( ErrorTag<TError> tag ) => new( false, error: tag.Error );
}