namespace Prospect.Engine;

public static class Time {
	// Ticks
	public static uint Tick => Entry.CurrentTick;
	public static uint TickRate => Entry.TickRate;
	public static float TickDelta => Entry.TickDelta;

	// Frames
	public static float FrameDelta => Entry.FrameDelta;

	/// <summary> Time since the game's startup </summary>
	public static float Now => CalculateTimeFromTick( Tick );

	internal static uint CalculateCurrentTick() => (uint)MathF.Ceiling( (float)Entry.RawGameTime.Elapsed.TotalSeconds * (float)TickRate );
	internal static float CalculateTimeFromTick( uint tick ) => (float)tick / (float)TickRate;
}
