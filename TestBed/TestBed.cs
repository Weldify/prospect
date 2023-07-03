using Prospect.Engine;
using System;

namespace Prospect.TestBed;

public class TestBed : IGame {
	static void Main() => Entry.Run<TestBed>();

	public GameOptions Options => GameOptions.Default with {
		TickRate = 60
	};

	public void Start() {
		Window.Title = "Prospect testbed";

		Input.MouseMode = MouseMode.Normal;
		Camera.FieldOfView = 70f;
	}

	public void Tick() {

	}

	public void Frame() {
		if ( Input.Pressed( Key.GraveAccent ) )
			Consoole.IsOpen = !Consoole.IsOpen;
	}

	public void Shutdown() {

	}
}
