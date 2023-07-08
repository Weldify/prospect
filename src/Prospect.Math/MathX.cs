using System;

namespace Prospect.Engine;

public static class MathX
{
    public const float DEG_TO_RAD = MathF.PI / 180f;
    public const float RAD_TO_DEG = 180f / MathF.PI;

    public static float ToRadians( this float f ) => f * DEG_TO_RAD;
    public static float ToDegrees( this float f ) => f * RAD_TO_DEG;

    public static float Remap( this float f, float oldMin, float oldMax, float newMin, float newMax )
        => ( ( f - oldMin ) / ( oldMax - oldMin ) * ( newMax - newMin ) ) + newMin;
}
