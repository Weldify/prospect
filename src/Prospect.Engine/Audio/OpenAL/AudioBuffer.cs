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
            // The following code was ripped out from a 500 year old resource i think
            // The commented out stuff makes it Mono16, by default its Stereo16
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