using System;
using Godot;

namespace Qwaitumin.LibreAutoTile.GUI.GodotBindings;

public static class GodotApi
{
  public static bool IsMouseOnElement(Control control)
    => control.GetGlobalRect().HasPoint(control.GetGlobalMousePosition());

  public static bool IsMouseOnElements(Control[] controls)
  {
    foreach (var control in controls)
      if (IsMouseOnElement(control)) return true;
    return false;
  }

  public static void FillOptionButtonWithEnum<T>(
      OptionButton enumOptionButton, T defaultValue) where T : struct, Enum
  {
    enumOptionButton.Clear();
    foreach (var enumValue in Enum.GetValues<T>())
      enumOptionButton.AddItem(enumValue.ToString());
    enumOptionButton.Select(Convert.ToInt32(defaultValue));
  }

  public static T AddChild<T>(Node parent, T child) where T : Node
  {
    parent.AddChild(child);
    return child;
  }

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