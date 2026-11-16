using Godot;
using LibreAutoTile.GUI.Settings;

namespace LibreAutoTile.GUI.Editor.View.Drawing;

public partial class TileProbability : Node2D
{
  private readonly Dictionary<Vector2I, Label> existingLabels = [];
  private readonly AppSettings settings;
  private Color fontColor = Colors.White;

  public TileProbability(AppSettings settings)
  {
    this.settings = settings;
  }

  public void UpdateFontColor(Color color)
  {
    fontColor = color;
    foreach (var (_, label) in existingLabels)
      label.LabelSettings = new() { FontColor = fontColor };
  }

  public void AddLabel(Vector2I position, double probability, int tileSize)
  {
    if (!existingLabels.TryGetValue(position, out var label))
    {
      label = new();
      existingLabels[position] = label;
      AddChild(label);
    }

    int referenceScaling = settings.WorldSettings.ImageScaling * 16;
    label.LabelSettings = new() { FontColor = fontColor };
    label.TextureFilter = TextureFilterEnum.Nearest;
    label.Size = new Vector2I(referenceScaling, referenceScaling);
    label.Scale = new Vector2I(tileSize / referenceScaling, tileSize / referenceScaling);
    label.GlobalPosition = position * tileSize;
    label.HorizontalAlignment = HorizontalAlignment.Center;
    label.VerticalAlignment = VerticalAlignment.Center;
    label.Text = $"{probability}";
  }

  public void ChangeLabelProbability(Vector2I position, double probability)
  {
    if (existingLabels.TryGetValue(position, out var label))
      label.Text = $"{probability}";
  }

  public void Clear()
  {
    foreach (var (_, label) in existingLabels)
      label.QueueFree();
    existingLabels.Clear();
  }
}
