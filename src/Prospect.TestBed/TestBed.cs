using Prospect.Engine;
using System;

namespace Prospect.TestBed;

public class TestBed : IGame
{
    static void Main() => Entry.Run<TestBed>();

    public GameOptions Options => GameOptions.Default with
    {
        TickRate = 60
    };

    public void Start()
    {
        Window.Title = "Prospect testbed";

        Input.MouseMode = MouseMode.Lock;
        Camera.FieldOfView = 70f;
        Camera.Transform = Transform.Zero;
    }

    public void Tick()
    {

    }

    readonly Model _sword = Model.Load( "../../../../assets/fish.mdl" );
    readonly Audio _fishAudio = Audio.Load( "../../../../assets/test.ogg" );
    Sound? _sound;
    Angles _lookAngles = Angles.Zero;

    public void Frame()
    {
        _lookAngles = ( _lookAngles + Input.LookDelta ).Wrapped;

        if ( _sound is null )
        {
            _sound = new();
            _sound.Audio = _fishAudio;
            _sound.Position = Vector3.Zero;
            _sound.Volume = 1f;
            _sound.Reach = 300f;
            _sound.DropStart = 0.9f;
            _sound.Looped = true;
            _sound.Play();
        }

        var forward = Convert.ToSingle( Input.Down( Key.W ) ) - Convert.ToSingle( Input.Down( Key.S ) );
        var right = Convert.ToSingle( Input.Down( Key.D ) ) - Convert.ToSingle( Input.Down( Key.A ) );

        Camera.Transform = Camera.Transform with { Rotation = Rotation.From( _lookAngles ) };

        Camera.Transform = Camera.Transform
            + Camera.Transform.Rotation.Forward * forward * RealTime.Delta
            + Camera.Transform.Rotation.Right * right * RealTime.Delta;

        if ( Input.ScrollDelta != 0f ) return;

        for (var x = -10; x < 10; x++ )
        {
            for (var y = -10; y < 10; y++ )
            {
                drawFish( new( x, y ) );
            }
        }
    }

    void drawFish(Vector2 gridPos)
    {
        var pos = new Vector3( gridPos.X, gridPos.Y, 0f );
        var almostRandom = gridPos.X + gridPos.Y;
        var transform = new Transform( pos, Rotation.From( new( ( RealTime.Now + almostRandom) * 10f, 0f, 0f ) ) );
        Graphics.DrawModel( _sword, transform );
    }

    public void Shutdown()
    {

    }
}
