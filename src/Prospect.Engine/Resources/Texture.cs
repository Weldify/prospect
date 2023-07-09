namespace Prospect.Engine;

sealed class TextureResource : Resource
{
    public string ImagePath = "";
    public TextureFilter Filter = TextureFilter.Nearest;
}