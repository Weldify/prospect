namespace Prospect.Engine;

[AttributeUsage( AttributeTargets.Class )]
public class ResourceAttribute : Attribute {
	public string FileExtension { get; private set; }
	public ResourceAttribute( string fileExtension ) => FileExtension = fileExtension;
}