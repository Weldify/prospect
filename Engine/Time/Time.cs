namespace Prospect.Engine;

public static class Time {
	public static uint TickRate => Entry.TickRate;
	public static float Delta => Entry.TickDelta;

	/// <summary> Time since the game's startup </summary>
	public static float Now => CalculateTimeFromTick( Tick );
	public static uint Tick => Entry.CurrentTick;

	internal static uint CalculateCurrentTick() => (uint)MathF.Ceiling( (float)Entry.RawGameTime.Elapsed.TotalSeconds * (float)TickRate );
	internal static float CalculateTimeFromTick( uint tick ) => (float)tick / (float)TickRate;
}
