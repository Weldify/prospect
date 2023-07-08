using Silk.NET.OpenAL;
using System;

namespace Prospect.Engine.OpenAL;

class AudioSource : IDisposable
{
    public AudioBuffer Buffer
    {
        set => _al.SetSourceProperty( _handle, SourceInteger.Buffer, value._handle );
    }

    readonly AL _al;
    readonly uint _handle;

    public AudioSource( AL al )
    {
        _al = al;
        _handle = _al.GenSource();
    }

    public void Play() => _al.SourcePlay( _handle );

    public void Dispose() => _al.DeleteSource( _handle );
}