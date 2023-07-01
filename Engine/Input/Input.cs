﻿using Silk.NET.Input;

namespace Prospect.Engine;

public static class Input {
	public static MouseMode MouseMode {
		get => Entry.Graphics.MouseMode;
		set => Entry.Graphics.MouseMode = value;
	}	

	public static Angles LookDelta => Entry.LookDelta;
	public static bool Pressed( Key key ) => Entry.HeldKeys.Contains( key ) && !Entry.PreviousHeldKeys.Contains( key );
	public static bool Down( Key key ) => Entry.HeldKeys.Contains( key );
}
