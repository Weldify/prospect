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

	static ResourceAttribute findAttribute( IResource resource ) =>
		resource.GetType().GetCustomAttributes().OfType<ResourceAttribute>().First();
	static ResourceAttribute findAttribute<T>() =>
		typeof( T ).GetCustomAttributes().OfType<ResourceAttribute>().First();


	static bool isPathValid( ResourceAttribute attrib, string path ) {
		var ext = Path.GetExtension( path );
		return ext == $".{attrib.FileExtension}";
	}

	public static T Get<T>( string path ) where T : IResource {
		if ( findAttribute<T>() is not ResourceAttribute attrib || !isPathValid( attrib, path ) )
			throw new Exception();

		var yaml = File.ReadAllText( path );
		return _deserializer.Deserialize<T>( yaml );
	}

	public static T GetOrCreate<T>( string path ) where T : IResource, new() {
		if ( File.Exists( path ) )
			return Get<T>( path );

		Write( new T(), path );
		return Get<T>( path );
	}

	public static void Write( this IResource resource, string path ) {
		if ( findAttribute( resource ) is not ResourceAttribute attrib || !isPathValid( attrib, path ) )
			throw new Exception();

		var yaml = _serializer.Serialize( resource );

		using StreamWriter writer = File.CreateText( path );
		writer.Write( yaml );
	}
}