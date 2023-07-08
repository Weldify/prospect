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

		Input.MouseMode = MouseMode.Lock;
		Camera.FieldOfView = 70f;
        Camera.Transform = Transform.Zero;
	}

	public void Tick() {

	}

	readonly Model _sword = Model.Load( "../../../../assets/fish.mdl" );
    readonly Audio _audio = Audio.Load( "../../../../assets/test.ogg" );
	Angles _lookAngles = Angles.Zero;
    TimeUntil _timeUntilGC;

	public void Frame() {
		_lookAngles = (_lookAngles + Input.LookDelta).Wrapped;

        if ( Input.Pressed( MouseButton.Left ) )
        {
            var sound = new Sound();
            sound.Audio = _audio;
            sound.Position = Vector3.Zero;
            sound.Reach = 5f;
            sound.DropStart = 0.9f;
            sound.Play();
        }

        if ( _timeUntilGC < 0f)
        {
            _timeUntilGC = 1f;
            GC.Collect();
        }

		var forward = Convert.ToSingle( Input.Down( Key.W ) ) - Convert.ToSingle( Input.Down( Key.S ) );
		var right = Convert.ToSingle( Input.Down( Key.D ) ) - Convert.ToSingle( Input.Down( Key.A ) );

		Camera.Transform = Camera.Transform with { Rotation = Rotation.From( _lookAngles ) };

		Camera.Transform = Camera.Transform
			+ Camera.Transform.Rotation.Forward * forward * Time.FrameDelta
			+ Camera.Transform.Rotation.Right * right * Time.FrameDelta;

		if ( Input.ScrollDelta != 0f ) return;

		var transform = new Transform( Vector3.Zero, Rotation.From( new( Time.Now * 10f, 0f, 0f ) ) );
		Graphics.DrawModel( _sword, transform );
	}

	public void Shutdown() {

	}
}
