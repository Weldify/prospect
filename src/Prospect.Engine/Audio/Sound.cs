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
            var oldPlaybackPosition = PlaybackPosition;

            Stop();

            _audio = value;
            _source.Buffer = _audio.BackendBuffer;

            if ( wasPlaying )
            {
                PlaybackPosition = oldPlaybackPosition;
                Play();
            }
        }
    }

    public bool IsPlaying => _source.State == AudioSourceState.Playing;
    public bool IsPaused => _source.State == AudioSourceState.Paused;

    public Vector3 Position
    {
        get => _source.Position;
        set => _source.Position = value;
    }

    /// <summary> 
    /// <c>[0f;1f]</c> 
    /// Volume of the sound - multiplier 
    /// </summary>
    public float Volume
    {
        get => _source.Volume;
        set => _source.Volume = value;
    }

    /// <summary>
    /// <c>[0.5f;2.0f]</c> 
    /// Pitch of the sound 
    /// </summary>
    public float Pitch
    {
        get => _source.Pitch;
        set => _source.Pitch = value;
    }

    /// <summary> Past this distance, the sound will be inaudible </summary>
    public float Reach
    {
        get => _source.Reach;
        set => _source.Reach = value;
    }

    /// <summary> 
    /// <c>[0f;1f]</c> 
    /// Dont start dropping volume until we pass this 
    /// </summary>
    public float DropStart
    {
        get => _source.DropStart;
        set => _source.DropStart = value;
    }

    /// <summary> Will the sound repeat after ending </summary>
    public bool Looped
    {
        get => _source.Looped;
        set => _source.Looped = value;
    }

    /// <summary>
    /// <c>[0f;1f]</c> 
    /// Current playback position of the sound
    /// </summary>
    public float PlaybackPosition
    {
        get => _source.PlaybackPosition;
        set => _source.PlaybackPosition = value;
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

    public void Pause()
    {
        _source.Pause();
        _ = _playingSounds.Remove( this );
    }

    public void Stop()
    {
        _source.Stop();
        _ = _playingSounds.Remove( this );
    }
}