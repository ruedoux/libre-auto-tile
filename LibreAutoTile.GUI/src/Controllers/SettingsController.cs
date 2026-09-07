using Godot;
using Qwaitumin.LibreAutoTile.Configuration.Models;
using Qwaitumin.LibreAutoTile.GUI.Views;

namespace Qwaitumin.LibreAutoTile.GUI.Controllers;

public class SettingsController
{
  private readonly EditorContext context;

  public SettingsController(EditorContext context)
  {
    this.context = context;
    var view = context.EditorScene.SettingsPanel;
    view.GridColorChanged += OnGridColorChanged;
    view.SelectionColorChanged += OnSelectionColorChanged;
    view.GuiColorChanged += OnGuiColorChanged;
    view.BackgroundColorChanged += OnBackgroundColorChanged;
    view.ProbabilityColorChanged += OnProbabilityColorChanged;
    view.TileSizeSubmitted += OnTileSizeChanged;
    view.FontSizeSubmitted += OnFontSizeChanged;
    view.ResolutionSelected += OnResolutionChanged;
    view.TileShapeSelected += OnTileShapeChanged;
  }

  public static void SeedViewFromModel(EditorContext context)
  {
    var appearance = context.AppearanceSettings;
    var view = context.EditorScene.SettingsPanel;
    view.SetTileSizeText(context.EditorData.TileSize.ToString());
    view.SetFontSizeText(appearance.FontSize.ToString());
    view.SetGuiColor(appearance.GuiColor);
    view.SetSelectionColor(appearance.SelectionColor);
    view.SetBackgroundColor(appearance.BackgroundColor);
    view.SetGridColor(appearance.GridColor);
    view.SetProbabilityColor(appearance.ProbabilityColor);
    view.SelectResolution(GetResolutionIndex(appearance.WindowSize));
    view.SelectTileShape(context.EditorData.TileShape);
    context.EditorScene.TilesPanel.SetWildcardIdText(context.EditorData.WildcardId?.ToString() ?? "");

    Settings.ApplyGuiColor(appearance.GuiColor);
    Settings.ApplyBackgroundColor(appearance.BackgroundColor);
  }

  private void OnGridColorChanged(Color color)
  {
    context.AppearanceSettings.GridColor = color;
    context.RedrawGrid();
  }

  private void OnSelectionColorChanged(Color color)
    => context.AppearanceSettings.SelectionColor = color;

  private void OnGuiColorChanged(Color color)
  {
    context.AppearanceSettings.GuiColor = color;
    Settings.ApplyGuiColor(color);
  }

  private void OnBackgroundColorChanged(Color color)
  {
    context.AppearanceSettings.BackgroundColor = color;
    Settings.ApplyBackgroundColor(color);
  }

  private void OnProbabilityColorChanged(Color color)
  {
    context.AppearanceSettings.ProbabilityColor = color;
    context.EditorScene.TileProbability.UpdateFontColor(color);
  }

  private void OnTileSizeChanged(string text)
  {
    if (!int.TryParse(text, out var size) || size <= 0)
      return;

    context.EditorData.TileSize = size;
    RedrawTiles();
  }

  private void OnFontSizeChanged(string text)
  {
    if (!int.TryParse(text, out var fontSize) || fontSize <= 0)
      return;

    context.AppearanceSettings.FontSize = fontSize;
    Settings.ApplyFontSize(fontSize);
  }

  private void OnResolutionChanged(long index)
  {
    var resolution = Settings.RESOLUTIONS[(int)index];
    context.AppearanceSettings.WindowSize = resolution;
    DisplayServer.WindowSetSize(resolution);
  }

  private void OnTileShapeChanged(TileShape tileShape)
  {
    context.EditorData.TileShape = tileShape;
    RedrawTiles();
  }

  private void RedrawTiles()
  {
    context.RedrawGrid();
    context.RedrawProbabilityLabels();
    context.RedrawBitmask();
    context.Probability.RedrawSelection();
  }

  private static int GetResolutionIndex(Vector2I windowSize)
  {
    for (int i = 0; i < Settings.RESOLUTIONS.Length; i++)
      if (Settings.RESOLUTIONS[i] == windowSize)
        return i;
    return Settings.DEFAULT_RESOLUTION_INDEX;
  }
}
