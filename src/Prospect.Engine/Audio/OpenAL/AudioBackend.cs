using NVorbis;
using Silk.NET.OpenAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prospect.Engine.OpenAL;

class AudioBackend : IAudioBackend
{
    AL _al;
    AudioSource _source;

    public AudioBackend()
    {
        _al = initAL();

        if ( _al.GetError() is var err && err != AudioError.NoError )
        {
            throw new Exception( $"OpenAL initialization failed: {err}" );
        }

        var testBuffer = new AudioBuffer( _al, "../../../../assets/test.ogg" );

        _source = new AudioSource( _al );
        _source.Buffer = testBuffer;
    }

    public IAudioSource CreateSource() => new AudioSource( _al );
    public IAudioBuffer LoadBuffer( string path ) => new AudioBuffer( _al, path );

    public void Update()
    {
        // Stop sounds that finished playing.
        // Allows them to be freed by GC
        foreach ( var sound in Sound._playingSounds.Where( s => !s.IsPlaying ) )
            sound.Stop();
    }

    AL initAL()
    {
        var alc = ALContext.GetApi();
        var al = AL.GetApi();

        unsafe
        {
            var device = alc.OpenDevice( "" );
            if ( device == null )
                throw new Exception( "Couldnt create AL device" );

            var context = alc.CreateContext( device, null );
            _ = alc.MakeContextCurrent( context );
        }

        return al;
    }
}