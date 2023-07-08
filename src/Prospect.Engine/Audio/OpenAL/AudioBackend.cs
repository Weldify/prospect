using NVorbis;
using Silk.NET.OpenAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prospect.Engine.OpenAL;

class AudioBackend : IAudioBackend
{
    AL _al;

    public AudioBackend()
    {
        _al = initAL();

        if ( _al.GetError() is var err && err != AudioError.NoError )
        {
            throw new Exception( $"OpenAL initialization failed: {err}" );
        }

        _al.DistanceModel( DistanceModel.LinearDistanceClamped );
    }

    public IAudioSource CreateSource() => new AudioSource( _al );
    public IAudioBuffer LoadBuffer( string path ) => new AudioBuffer( _al, path );

    public void Frame()
    {
        updateListener();
        stopFinishedSounds();
    }

    void updateListener()
    {
        var cameraPosition = Camera.Transform.Position;
        cameraPosition.Z = -cameraPosition.Z; // OpenAL is right handed, we are left handed

        var cameraForward = Camera.Transform.Rotation.Forward;
        var cameraUp = Camera.Transform.Rotation.Up;

        // Convert to OpenAL coordinate system
        cameraForward.Z = -cameraForward.Z;
        cameraUp.Z = -cameraUp.Z;

        var directions = new float[]
        {
            cameraForward.X, cameraForward.Y, cameraForward.Z,
            cameraUp.X, cameraUp.Y, cameraUp.Z
        };

        _al.SetListenerProperty( ListenerVector3.Position, cameraPosition );
        unsafe
        {
            fixed (float* dirs = directions)
            {
                _al.SetListenerProperty( ListenerFloatArray.Orientation, dirs );
            }
        }

        Console.WriteLine( cameraPosition );
    }

    void stopFinishedSounds()
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