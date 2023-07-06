namespace Prospect.Engine;

public struct Status {
	public static OkTag Ok() => new();
	public static OkTag<T> Ok<T>( T value ) => new( value );
	public static ErrorTag Fail() => new();
	public static ErrorTag<T> Fail<T>( T error ) => new( error );

	public bool IsOk;
	public bool IsError => !IsOk;

	Status( bool isOk ) => IsOk = isOk;

	public static implicit operator Status( OkTag tag ) => new( true );
	public static implicit operator Status( ErrorTag tag ) => new( false );
}

public struct Status<TError> {
	public readonly static Status<TError> Ok = new( true );
	public readonly static Status<TError> Fail = new( false );

	public TError Error => IsError ? _error : throw new Exception( "Unwrapped ok status!" );
	public bool IsOk;
	public bool IsError => !IsOk;

	TError _error;

#nullable disable
	Status( bool isOk, TError error = default ) {
#nullable enable
		IsOk = isOk;
		_error = error;
	}

	public static implicit operator Status<TError>( TError error ) => new( false, error );
	public static implicit operator Status<TError>( OkTag tag ) => new( true );
	public static implicit operator Status<TError>( ErrorTag<TError> tag ) => new( false, tag.Error );
}