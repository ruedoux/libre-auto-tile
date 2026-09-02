using Godot;

namespace Qwaitumin.LibreAutoTile.GUI.Views;

public partial class TileProbability : Node2D
{
  private readonly Dictionary<Vector2I, Label> existingLabels = [];
  private Color fontColor = Colors.White;

  public void UpdateFontColor(Color color)
  {
    fontColor = color;
    foreach (var (_, label) in existingLabels)
      label.LabelSettings = new() { Font = Settings.FONT, FontColor = fontColor };
  }

  public void AddLabel(Vector2I position, double probability, int tileSize)
  {
    if (!existingLabels.TryGetValue(position, out var label))
    {
      label = new();
      existingLabels[position] = label;
      AddChild(label);
    }

    int referenceScaling = Settings.IMAGE_SCALING * 16;
    label.LabelSettings = new() { Font = Settings.FONT, FontColor = fontColor };
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
