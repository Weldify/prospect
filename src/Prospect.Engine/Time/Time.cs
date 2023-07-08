using System;

namespace Prospect.Engine;

public static class Time
{
    // Ticks
    public static uint Tick => Entry.CurrentTick;
    public static uint TickRate => Entry.TickRate;
    public static float TickDelta => Entry.TickDelta;

    /// <summary> Time since startup, aligned to the current tick </summary>
    public static float Now => calculateTimeFromTick( Tick );

    internal static uint calculateCurrentTick() => (uint)MathF.Ceiling( (float)Entry.RawGameTime.Elapsed.TotalSeconds * (float)TickRate );
    internal static float calculateTimeFromTick( uint tick ) => (float)tick / (float)TickRate;
}
