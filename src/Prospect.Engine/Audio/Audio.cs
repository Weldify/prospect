using System;
using System.Collections.Generic;
using System.IO;

namespace Prospect.Engine;

public sealed class Audio
{
    internal readonly static Dictionary<string, Audio> _cache = new();

    public float Length { get; private set; }

    internal IAudioBuffer BackendBuffer { get; private set; } = null!;

    public static Audio Load( string path )
    {
        if ( _cache.TryGetValue( path, out var aud ) )
            return aud;

        var audio = new Audio();

        audio.BackendBuffer = Entry.Audio.LoadBuffer( path );
        audio.Length = audio.BackendBuffer.Length;

        _cache[ path ] = audio;
        return audio;
    }
}