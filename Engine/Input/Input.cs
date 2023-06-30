namespace Prospect.Engine;

public static class Input {
	public static bool Pressed( Key key ) => Entry.HeldKeys.Contains( key ) && !Entry.PreviousHeldKeys.Contains( key );
	public static bool Down( Key key ) => Entry.HeldKeys.Contains( key );
}
