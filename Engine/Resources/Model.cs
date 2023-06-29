using YamlDotNet.Serialization;

namespace Prospect.Engine;

sealed class ModelResource : IResource {
	public string MeshPath = "";
	public string TexturePath = "";
}
