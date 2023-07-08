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
    Sound? _sound;
	Angles _lookAngles = Angles.Zero;

	public void Frame() {
		_lookAngles = (_lookAngles + Input.LookDelta).Wrapped;

        if (_sound is null)
        {
            _sound = new();
            _sound.Audio = _audio;
            _sound.Position = Vector3.Zero;
            _sound.Volume = 1f;
            _sound.Reach = 5f;
            _sound.Pitch = 0.5f;
            _sound.DropStart = 0.9f;
            _sound.Looped = true;
            _sound.Play();
        }

        _sound.PlaybackPosition = 0.5f;
        //_sound.Pitch = MathF.Abs(MathF.Sin(Time.Now)).Remap(0f, 1f, 0.5f, 2f);

		var forward = Convert.ToSingle( Input.Down( Key.W ) ) - Convert.ToSingle( Input.Down( Key.S ) );
		var right = Convert.ToSingle( Input.Down( Key.D ) ) - Convert.ToSingle( Input.Down( Key.A ) );

		Camera.Transform = Camera.Transform with { Rotation = Rotation.From( _lookAngles ) };

		Camera.Transform = Camera.Transform
			+ Camera.Transform.Rotation.Forward * forward * Time.FrameDelta
			+ Camera.Transform.Rotation.Right * right * Time.FrameDelta;

		if ( Input.ScrollDelta != 0f ) return;

		var transform = new Transform( Vector3.Zero, Rotation.From( new( Time.Now * 200f, 0f, 0f ) ) );
		Graphics.DrawModel( _sword, transform );
	}

	public void Shutdown() {

	}
}
