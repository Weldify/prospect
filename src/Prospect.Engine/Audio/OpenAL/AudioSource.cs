using Silk.NET.OpenAL;
using System;

namespace Prospect.Engine.OpenAL;

class AudioSource : IAudioSource, IDisposable
{
    public IAudioBuffer? Buffer
    {
        get => _buffer;
        set
        {
            _buffer = value as AudioBuffer;
            var bufferHandle = _buffer?._handle ?? 0;
            _al.SetSourceProperty( _handle, SourceInteger.Buffer, bufferHandle );
        }
    }

    public AudioSourceState State
    {
        get
        {
            _al.GetSourceProperty( _handle, GetSourceInteger.SourceState, out var state );

            return (SourceState)state switch
            {
                SourceState.Playing => AudioSourceState.Playing,
                SourceState.Paused => AudioSourceState.Paused,
                _ => AudioSourceState.Stopped,
            };
        }
    }

    public Vector3 Position
    {
        get => _position;
        set
        {
            _position = value;
            _al.SetSourceProperty( _handle, SourceVector3.Position, _position );
        }
    }

    public float Reach
    {
        get => _reach;
        set
        {
            _reach = Math.Max( 0f, value );

            _al.SetSourceProperty( _handle, SourceFloat.MaxDistance, _reach );
            updateReferenceDistance();
        }
    }

    public float DropStart
    {
        get => _dropStart;
        set
        {
            _dropStart = Math.Clamp(value, 0f, 1f);
            updateReferenceDistance();
        }
    }

    public bool Looped
    {
        get => _looped;
        set
        {
            _looped = value;
            _al.SetSourceProperty( _handle, SourceBoolean.Looping, _looped );
        }
    }

    readonly AL _al;
    readonly uint _handle;

    AudioBuffer? _buffer;
    Vector3 _position;
    float _reach;
    float _dropStart;
    bool _looped;

    public AudioSource( AL al )
    {
        _al = al;
        _handle = _al.GenSource();

        _al.SetSourceProperty( _handle, SourceFloat.Gain, 1f );
        _al.SetSourceProperty( _handle, SourceFloat.Pitch, 1f );
        _al.SetSourceProperty( _handle, SourceFloat.RolloffFactor, 1f );

        Position = Vector3.Zero;
        Reach = 1f;
        DropStart = 0f;
        Looped = false;
    }

    public void Play() => _al.SourcePlay( _handle );
    public void Stop() => _al.SourceStop( _handle );

    public void Dispose() => _al.DeleteSource( _handle );

    void updateReferenceDistance()
        => _al.SetSourceProperty( _handle, SourceFloat.ReferenceDistance, _reach * _dropStart );
}