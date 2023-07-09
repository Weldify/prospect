namespace Prospect.Engine;

enum TextureFilter
{
    Linear,
    Nearest
}

sealed class TextureResource : Resource
{
    public string ImagePath = "";
    public TextureFilter Filter = TextureFilter.Nearest;
}