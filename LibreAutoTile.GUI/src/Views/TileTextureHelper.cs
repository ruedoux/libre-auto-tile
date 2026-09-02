using Godot;

namespace Qwaitumin.LibreAutoTile.GUI.Views;

public static class TileTextureHelper
{
  public static Texture2D GetTileTexture(
    TileSetAtlasSource source, Vector2I tilePosition, Vector2I tileSize)
  {
    var image = source.Texture.GetImage();
    var tileImage = Image.CreateEmpty(tileSize.X, tileSize.Y, false, Image.Format.Rgba8);
    if (image.GetSize().X < tileSize.X || image.GetSize().Y < tileSize.Y)
      return ImageTexture.CreateFromImage(tileImage);

    tileImage.BlitRect(image, new(tilePosition * tileSize, tileSize), Vector2I.Zero);
    return ImageTexture.CreateFromImage(tileImage);
  }
}
