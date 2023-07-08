using System;

namespace Prospect.Engine;

public sealed class Sound
{
    public Audio? Audio
    {
        get => _audio;
        set
        {
            if ( value is null )
            {
                _audio = null;
                // Stop playing & reset time here
                return;
            }

            _audio = value;
            _source.Buffer = _audio.BackendBuffer;
        }
    }

    readonly IAudioSource _source;
    Audio? _audio;

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

    public void Play() => _source.Play();
    public void Stop() => _source.Stop();
}