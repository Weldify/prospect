using System;

namespace Prospect.Engine;

public static class Time
{
    // Ticks
    public static uint Tick => Entry.CurrentTick;
    public static uint TickRate => Entry.TickRate;
    public static float TickDelta => Entry.TickDelta;

    // Frames
    public static float FrameDelta => Entry.FrameDelta;

    /// <summary> Time since the game's startup </summary>
    public static float Now => Entry.LoopState switch
    {
        GameLoopState.Ticking => calculateTimeFromTick( Tick ),
        GameLoopState.Framing or _ => (float)Entry.RawGameTime.Elapsed.TotalSeconds,
    };


    internal static uint calculateCurrentTick() => (uint)MathF.Ceiling( (float)Entry.RawGameTime.Elapsed.TotalSeconds * (float)TickRate );
    internal static float calculateTimeFromTick( uint tick ) => (float)tick / (float)TickRate;
}
