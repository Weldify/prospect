using Silk.NET.OpenAL;
using System;

namespace Prospect.Engine.OpenAL;

class AudioSource : IAudioSource, IDisposable
{
    public IAudioBuffer? Buffer
    {
        get => _buffer;
        set {
            _buffer = value as AudioBuffer;
            var bufferHandle = _buffer?._handle ?? 0;
            _al.SetSourceProperty( _handle, SourceInteger.Buffer, bufferHandle );
        }
    }

    readonly AL _al;
    readonly uint _handle;
    AudioBuffer? _buffer;

    public AudioSource( AL al )
    {
        _al = al;
        _handle = _al.GenSource();
    }

    public void Play() => _al.SourcePlay( _handle );
    public void Stop() => _al.SourceStop( _handle );

    public void Dispose() => _al.DeleteSource( _handle );
}