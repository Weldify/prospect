using System;
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
            if ( value is null )
            {
                Stop();
                _audio = null;

                return;
            }

            _audio = value;
            _source.Buffer = _audio.BackendBuffer;
        }
    }

    public bool IsPlaying => _isPlaying && _sinceStartedPlaying < _audio!.Length;

    readonly IAudioSource _source;
    Audio? _audio;

    TimeSince _sinceStartedPlaying;
    bool _isPlaying;

    public Sound( Audio audio )
    {
        _source = Entry.Audio.CreateSource();
        Audio = audio;
    }

    ~Sound()
    {
        Stop();
        _source.Dispose();
    }

    public void Play()
    {
        // We dont have audio set. It's appropriate to do nothing here
        if ( _audio is null ) return;

        _isPlaying = true;
        _sinceStartedPlaying = 0f;

        // Prevent this sound from being GCed while playing
        // The backend handles removing sounds that stopped playing
        _ = _playingSounds.Add( this );

        _source.Play();
    }
    public void Stop()
    {
        _isPlaying = false;
        _source.Stop();

        _ = _playingSounds.Remove( this );
    }
}