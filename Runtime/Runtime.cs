using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Prospect.Engine;

namespace Prospect.Runtime;

class Runtime {
	static void Main() {
		Console.WriteLine( "I shart" );

		var gameAssembly = Assembly.LoadFile( Path.GetFullPath( "game.dll" ) );

		var games = gameAssembly.GetExportedTypes().Where( t => t.GetInterfaces().Contains( typeof( IGame ) ) );
		if ( !games.Any() )
			throw new Exception( "Assembly has no games!" );

		Entry.Run( games.First() );
	}
}
