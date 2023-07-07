using YamlDotNet.Serialization;

namespace Prospect.Engine;

public abstract class Resource
{
    [YamlIgnore]
    public string Directory { get; internal set; } = null!;
}