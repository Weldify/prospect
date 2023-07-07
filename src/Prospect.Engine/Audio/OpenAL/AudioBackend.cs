using NVorbis;
using Silk.NET.OpenAL;
using System;
using System.Collections.Generic;

namespace Prospect.Engine.OpenAL;

class AudioBackend : IAudioBackend
{
    AL _al;
    uint _source;
    uint _buffer;

    public AudioBackend()
    {
        _al = initAL();

        if (_al.GetError() is var err && err != AudioError.NoError)
        {
            throw new Exception( $"OpenAL initialization failed: {err}" );
        }

        _source = _al.GenSource();
        _buffer = _al.GenBuffer();

        // Decoding the sound
        using var fart = new VorbisReader( "../../../../assets/test.ogg" );

        var floatBuffer = new float[ 1024 ];
        var result = new List<byte>();

        var totalSamples = 0;
        int count;
        while ((count = fart.ReadSamples( floatBuffer, 0, floatBuffer.Length)) > 0)
        {
            totalSamples += count;

            // Black magic
            for ( var i = 0; i < count; i++ )
            {
                var temp = (short)( 32767f * floatBuffer[ i ] );
                if ( temp > 32767 )
                {
                    result.Add( 0xFF );
                    result.Add( 0x7F );
                }
                else if ( temp < -32768 )
                {
                    result.Add( 0x80 );
                    result.Add( 0x00 );
                }
                result.Add( (byte)temp );
                result.Add( (byte)( temp >> 8 ) );
            }
        }

        var buffer = result.ToArray();

        unsafe
        {
            fixed ( byte* data = buffer )
            {
                _al.BufferData( _buffer, BufferFormat.Stereo16, data, buffer.Length, fart.SampleRate );
            }
        }
        _al.SetSourceProperty( _source, SourceInteger.Buffer, _buffer );
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

    public void PlayFardd() => _al.SourcePlay( _source );
}