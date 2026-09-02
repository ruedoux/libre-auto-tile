using Godot;

namespace Qwaitumin.LibreAutoTile.GUI.Views.Presentation;

public sealed class AppearanceSettings
{
  public int FontSize { get; set; } = 32;
  public Vector2I WindowSize { get; set; } = new(1920, 1080);

  public Color GridColor { get; set; } = Colors.Orange;
  public Color SelectionColor { get; set; } = Colors.White;
  public Color GuiColor { get; set; } = new(0.2f, 0.2f, 0.2f);
  public Color BackgroundColor { get; set; } = new(0.3f, 0.3f, 0.3f);
  public Color ProbabilityColor { get; set; } = Colors.White;
}
