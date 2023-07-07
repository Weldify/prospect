namespace Prospect.Engine;

public struct GameOptions {
	public readonly static GameOptions Default = new();

	public uint TickRate = 60;

	public GameOptions() { }
}
