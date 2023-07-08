using System;

namespace Prospect.Engine;

interface IAudioBackend
{
    IAudioSource CreateSource();
    IAudioBuffer LoadBuffer( string path );

    void Update();
}