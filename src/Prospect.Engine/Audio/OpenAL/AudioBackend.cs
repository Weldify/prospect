using NVorbis;
using Silk.NET.OpenAL;
using System;
using System.Collections.Generic;

namespace Prospect.Engine.OpenAL;

class AudioBackend : IAudioBackend
{
    AL _al;
    AudioSource _source;

    public AudioBackend()
    {
        _al = initAL();

        if (_al.GetError() is var err && err != AudioError.NoError)
        {
            throw new Exception( $"OpenAL initialization failed: {err}" );
        }

        var testBuffer = new AudioBuffer( _al, "../../../../assets/test.ogg" );

        _source = new AudioSource( _al );
        _source.Buffer = testBuffer;
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

    public void PlayFardd() => _source.Play();
}