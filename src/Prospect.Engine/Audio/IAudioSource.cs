using System;

namespace Prospect.Engine;

interface IAudioSource : IDisposable
{
    public IAudioBuffer? Buffer { get; set; }
    public Vector3 Position { get; set; }
    public float Volume { get; set; }
    public float Pitch { get; set; }
    public float Reach { get; set; }
    public float DropStart { get; set; }
    public bool Looped { get; set; }

    public AudioSourceState State { get; }

    void Play();
    void Stop();
}