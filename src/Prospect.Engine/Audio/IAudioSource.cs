using System;

namespace Prospect.Engine;

interface IAudioSource : IDisposable
{
    public IAudioBuffer? Buffer { get; set; }

    void Play();
    void Stop();
}