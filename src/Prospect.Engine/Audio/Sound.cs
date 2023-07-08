﻿using System;
using System.Collections.Generic;

namespace Prospect.Engine;

public sealed class Sound
{
    internal static HashSet<Sound> _playingSounds = new();

    public Audio? Audio
    {
        get => _audio;
        set
        {
            // If value is null, stop playing the audio.
            if ( value is null )
            {
                Stop();
                _audio = null;
                _source.Buffer = null;

                return;
            }

            // If value isn't null, swap out the audio but keep playing
            var wasPlaying = IsPlaying;

            Stop();

            _audio = value;
            _source.Buffer = _audio.BackendBuffer;

            if ( wasPlaying ) Play();
        }
    }

    public bool IsPlaying => _source.State == AudioSourceState.Playing;
    public Vector3 Position
    {
        get => _source.Position;
        set => _source.Position = value;
    }

    readonly IAudioSource _source;
    Audio? _audio;

    public Sound() => _source = Entry.Audio.CreateSource();

    ~Sound()
    {
        Stop();
        _source.Dispose();
    }

    public void Play()
    {
        // We dont have audio set. It's appropriate to do nothing here
        if ( _audio is null ) return;

        // Prevent this sound from being GCed while playing
        // The backend handles removing sounds that stopped playing
        _ = _playingSounds.Add( this );

        _source.Play();
    }
    public void Stop()
    {
        _source.Stop();

        _ = _playingSounds.Remove( this );
    }
}