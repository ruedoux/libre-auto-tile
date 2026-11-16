namespace LibreAutoTile.GUI.Editor.Model;

public sealed class DisplayOptions
{
  public bool Native { get; set; }
  public int Width { get; set; }
  public int Height { get; set; }

  public static DisplayOptions CreateDefault(int width, int height)
    => new() { Width = width, Height = height };
}
