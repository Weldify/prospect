namespace Prospect.Engine;

public readonly struct Result {
	// All the constructors are here to make the API nice
	public static Result Ok() => new( true );
	public static Result Fail() => new( false );
	public static Result<TV> Ok<TV>( TV value ) => new( true, value );
	public static Result<TV> Fail<TV>() => new( false );
	public static Result<TV, TE> Ok<TV, TE>( TV value ) => new( true, value );
	public static Result<TV, TE> Fail<TV, TE>( TE error ) => new( false, error: error );

	public readonly bool IsOk;
	public bool Failed => !IsOk;

	Result( bool isOk ) => IsOk = isOk;
}

public readonly struct Result<TValue> {
	public readonly bool IsOk;
	public bool Failed => !IsOk;

	public TValue Value => IsOk ? _value : throw new Exception( "Tried accessing Result.Value, but Result is Error" );

	readonly TValue _value;

#nullable disable
	internal Result( bool isOk, TValue value = default ) {
#nullable enable
		IsOk = isOk;
		_value = value;
	}
}

public readonly struct Result<TValue, TError> {
	public readonly bool IsOk;
	public bool Failed => !IsOk;

	public TValue Value => IsOk ? _value : throw new Exception( "Tried accessing Result.Value, but Result is Error" );
	public TError Error => Failed ? _error : throw new Exception( "Tried accessing Result.Error, but Result is Ok" );

	readonly TValue _value;
	readonly TError _error;

#nullable disable
	internal Result( bool isOk, TValue value = default, TError error = default ) {
#nullable enable
		IsOk = isOk;
		_value = value;
		_error = error;
	}
}