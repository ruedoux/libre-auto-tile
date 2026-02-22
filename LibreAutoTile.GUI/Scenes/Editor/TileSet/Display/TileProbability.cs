using Godot;

namespace Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.TileSet.Display;

public partial class TileProbability : Node2D
{
  private static readonly Font FONT = ResourceLoader.Load<Font>("uid://2cika1jlvtuc");
  private readonly Dictionary<Vector2I, Label> existingLabels = [];
  private Color fontColor = Colors.White;

  public void UpdateFontColor(Color color)
  {
    fontColor = color;
    foreach (var (_, label) in existingLabels)
      label.LabelSettings = new() { Font = FONT, FontColor = fontColor };
  }

  public Vector2I[] GetAllPositions()
    => [.. existingLabels.Keys];

  public void AddLabel(Vector2I position, double probability, int tileSize)
  {
    if (!existingLabels.TryGetValue(position, out var label))
    {
      label = new();
      existingLabels[position] = label;
      AddChild(label);
    }

    int referenceScaling = Editor.IMAGE_SCALING * 16;
    label.LabelSettings = new() { Font = FONT, FontColor = fontColor };
    label.TextureFilter = TextureFilterEnum.Nearest;
    label.Size = new Vector2I(referenceScaling, referenceScaling);
    label.Scale = new Vector2I(tileSize / referenceScaling, tileSize / referenceScaling);
    label.GlobalPosition = position * tileSize;
    label.HorizontalAlignment = HorizontalAlignment.Center;
    label.VerticalAlignment = VerticalAlignment.Center;
    label.Text = $"{probability}";
  }

  public void AddProbability(Vector2I position, double probability)
  {
    if (existingLabels.TryGetValue(position, out var label))
      label.Text = $"{Math.Clamp(InputSanitizer.SanitizeDouble(label.Text) + probability, 0, double.MaxValue)}";
  }

  public void ChangeLabelProbability(Vector2I position, double probability)
  {
    if (existingLabels.TryGetValue(position, out var label))
      label.Text = $"{probability}";
  }

  public void ChangeLabelSize(Vector2I position, int tileSize)
  {
    if (existingLabels.TryGetValue(position, out var label))
    {
      int referenceScaling = Editor.IMAGE_SCALING * 16;
      label.Scale = new Vector2I(tileSize / referenceScaling, tileSize / referenceScaling);
      label.GlobalPosition = position * tileSize;
    }
  }

  public void RemoveLabel(Vector2I position)
  {
    if (!existingLabels.TryGetValue(position, out var label))
      return;
    existingLabels.Remove(position);
    label.QueueFree();
  }

  public void Clear()
  {
    foreach (var (_, label) in existingLabels)
      label.QueueFree();
    existingLabels.Clear();
  }
}