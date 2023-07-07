using System;
using System.IO;
using System.Reflection;
using YamlDotNet;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Prospect.Engine;

public static class Resources
{
    readonly static ISerializer _serializer =
        new SerializerBuilder()
        .WithNamingConvention( PascalCaseNamingConvention.Instance )
        .WithIndentedSequences()
        .Build();

    readonly static IDeserializer _deserializer =
        new DeserializerBuilder()
        .WithNamingConvention( PascalCaseNamingConvention.Instance )
        .Build();

    public static T? Get<T>( string path ) where T : Resource
    {
        if ( !File.Exists( path ) )
            return default;

        var yaml = File.ReadAllText( path );
        var res = _deserializer.Deserialize<T>( yaml );
        res.Directory = Path.Combine( path, ".." );

        return res;
    }

    public static T GetOrCreate<T>( string path ) where T : Resource, new()
    {
        if ( Get<T>( path ) is T resource )
            return resource;

        var newResource = new T()
        {
            Directory = Path.Combine( path, ".." ),
        };

        newResource.Write( path );

        return newResource;
    }

    public static string Serialize( this Resource resource )
        => _serializer.Serialize( resource );

    public static void Write( this Resource resource, string path )
        => File.WriteAllText( path, resource.Serialize() );
}
