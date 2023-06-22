using Prospect.Engine;

namespace Prospect.Editor;

[Resource( "eds" )]
class EditorSettings : IResource {
	public List<string> Projects = new();
	public bool Owgewp = false;
}