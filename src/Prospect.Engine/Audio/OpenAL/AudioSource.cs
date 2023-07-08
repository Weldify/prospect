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

    public Vector3 Position
    {
        get => _position;
        set
        {
            _position = value;

            var alPos = _position;
            alPos.Z = -alPos.Z; // OpenAL is right handed, we are left handed

            _al.SetSourceProperty( _handle, SourceVector3.Position, alPos );
        }
    }

    public float Reach
    {
        get => _reach;
        set
        {
            _reach = Math.Max( 0f, value );

            _al.SetSourceProperty( _handle, SourceFloat.MaxDistance, _reach );
        }
    }

    readonly AL _al;
    readonly uint _handle;

    AudioBuffer? _buffer;
    Vector3 _position;
    float _reach;

    public AudioSource( AL al )
    {
        _al = al;
        _handle = _al.GenSource();

        Position = Vector3.Zero;
        Reach = 1f;
    }

    public void Play() => _al.SourcePlay( _handle );
    public void Stop() => _al.SourceStop( _handle );

    public void Dispose() => _al.DeleteSource( _handle );
}