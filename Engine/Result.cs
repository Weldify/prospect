namespace Prospect.Engine;

public readonly struct Result<TValue> {
	public static Result<TValue> Ok( TValue value ) => new( true, value );
	public static Result<TValue> Fail() => new( false );

	public readonly bool IsOk;
	public bool Failed => !IsOk;

	public TValue Value => IsOk ? _value : throw new Exception( "Tried accessing Result.Value, but Result is Error" );

	readonly TValue _value;

#nullable disable
	Result( bool isOk, TValue value = default ) {
#nullable enable
		IsOk = isOk;
		_value = value;
	}
}

public readonly struct Result<TValue, TError> {
	public static Result<TValue, TError> Ok( TValue value ) => new( true, value );
	public static Result<TValue, TError> Fail( TError error ) => new( false, error: error );

	public readonly bool IsOk;
	public bool Failed => !IsOk;

	public TValue Value => IsOk ? _value : throw new Exception( "Tried accessing Result.Value, but Result is Error" );
	public TError Error => Failed ? _error : throw new Exception( "Tried accessing Result.Error, but Result is Ok" );

	readonly TValue _value;
	readonly TError _error;

#nullable disable
	Result( bool isOk, TValue value = default, TError error = default ) {
#nullable enable
		IsOk = isOk;
		_value = value;
		_error = error;
	}
}