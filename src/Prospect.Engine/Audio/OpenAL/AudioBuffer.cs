using NVorbis;
using Silk.NET.OpenAL;
using System;
using System.Collections.Generic;

namespace Prospect.Engine.OpenAL;

class AudioBuffer : IAudioBuffer, IDisposable
{
    public float Length { get; private set; }

    readonly AL _al;
    internal readonly uint _handle;

    public AudioBuffer( AL al, string path )
    {
        _al = al;
        _handle = _al.GenBuffer();

        using var reader = new VorbisReader( path );

        var floatBuffer = new float[ 1024 ];
        var result = new List<byte>();

        int count;
        while ( ( count = reader.ReadSamples( floatBuffer, 0, floatBuffer.Length ) ) > 0 )
        {
            // TODO: BAD BAD BAD BAD BAD 
            // The following code was ripped out from a 500 year old resource 
            // This decodes the samples into a byte[]
            // Normally its fine, but it decodes it in Stereo16
            // OpenAL attenuation doesnt work with Stereo16, so I had to convert it to Mono16
            // But I have no CLUE IN THE SLIGHTEST what I am doing
            // This code probably discards one of the channels. Fucking sucks.
            for ( var i = 0; i < count; i++ )
            {
                var temp = (short)( 32767f * floatBuffer[ i ] );
                if ( temp > 32767 )
                {
                    //result.Add( 0xFF );
                    result.Add( 0x7F );
                }
                else if ( temp < -32768 )
                {
                    //result.Add( 0x80 );
                    result.Add( 0x00 );
                }
                //result.Add( (byte)temp );
                result.Add( (byte)( temp >> 8 ) );
            }
        }

        var buffer = result.ToArray();

        unsafe
        {
            fixed ( byte* data = buffer )
            {
                _al.BufferData( _handle, BufferFormat.Mono16, data, buffer.Length, reader.SampleRate );
            }
        }

        Length = (float)reader.TotalTime.TotalSeconds;
    }

    public void Dispose() => _al.DeleteBuffer( _handle );
}