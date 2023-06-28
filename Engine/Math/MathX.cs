namespace Prospect.Engine;

public static class MathX {
	const float _PI_DEG = MathF.PI / 180f;

	public static float ToRadians( this float f ) => f * _PI_DEG;
	public static float ToDegrees( this float f ) => f / _PI_DEG;
}
