using System.IO;
using System.Reflection;
using YamlDotNet;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Prospect.Engine;

public static class Resources {
	readonly static ISerializer _serializer =
		new SerializerBuilder()
		.WithNamingConvention( PascalCaseNamingConvention.Instance )
		.Build();

	readonly static IDeserializer _deserializer =
		new DeserializerBuilder()
		.WithNamingConvention( PascalCaseNamingConvention.Instance )
		.Build();

	public static T? Get<T>( string path ) where T : IResource {
		if ( !File.Exists( path ) )
			return default;

		var yaml = File.ReadAllText( path );
		return _deserializer.Deserialize<T>( yaml );
	}

	public static T GetOrCreate<T>( string path ) where T : IResource, new() {
		if ( Get<T>( path ) is T resource )
			return resource;

		var newResource = new T();
		newResource.Write( path );

		return newResource;
	}

	public static string Serialize( this IResource resource ) {
		var yaml = _serializer.Serialize( resource );
		return yaml;
	}

	public static void Write( this IResource resource, string path ) {
		File.WriteAllText( path, resource.Serialize() );
	}
}
