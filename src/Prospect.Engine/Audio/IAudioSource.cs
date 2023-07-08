using System;

namespace Prospect.Engine;

interface IAudioSource : IDisposable
{
    public IAudioBuffer? Buffer { get; set; }
    public Vector3 Position { get; set; }
    public float Reach { get; set; }

    void Play();
    void Stop();
}