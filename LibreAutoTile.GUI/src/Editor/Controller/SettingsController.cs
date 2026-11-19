using Godot;
using LibreAutoTile.Configuration.Models;
using LibreAutoTile.GUI.Editor.Model;
using LibreAutoTile.GUI.Editor.View;
using LibreAutoTile.GUI.Settings;

namespace LibreAutoTile.GUI.Editor.Controller;

public class SettingsController
{
  private readonly EditorContext context;
  private readonly EditorModel model;
  private readonly EditorView view;
  private readonly AppSettings settings;
  private readonly EditorRedrawer redrawer;

  public SettingsController(EditorContext context)
  {
    this.context = context;
    model = context.Model;
    view = context.View;
    settings = context.Settings;
    redrawer = context.Redrawer;

    var panel = view.ToolsPanel.SettingsPanel;
    panel.GridColorChanged += OnGridColorChanged;
    panel.SelectionColorChanged += OnSelectionColorChanged;
    panel.GuiColorChanged += OnGuiColorChanged;
    panel.BackgroundColorChanged += OnBackgroundColorChanged;
    panel.ProbabilityColorChanged += OnProbabilityColorChanged;
    panel.TileSizeSubmitted += OnTileSizeChanged;
    panel.FontSizeSubmitted += OnFontSizeChanged;
    panel.ResolutionSelected += OnResolutionChanged;
    panel.TileShapeSelected += OnTileShapeChanged;
    panel.ProjectRootSelected += OnProjectRootChanged;

    SeedViewFromModel();
  }

  private void SeedViewFromModel()
  {
    var uiSettings = settings.UiSettings;
    var editorSettings = settings.EditorSettings;
    var panel = view.ToolsPanel.SettingsPanel;
    panel.SetTileSizeText(model.TileSize.ToString());
    panel.SetFontSizeText(uiSettings.FontSize.ToString());
    panel.SetProjectRootText(model.ImageRootPath);
    panel.SetGuiColor(uiSettings.GuiColor);
    panel.SetSelectionColor(editorSettings.SelectionColor);
    panel.SetBackgroundColor(uiSettings.BackgroundColor);
    panel.SetGridColor(editorSettings.GridColor);
    panel.SetProbabilityColor(editorSettings.ProbabilityColor);
    panel.SelectResolution(settings.DisplayOptions);
    panel.SelectTileShape(model.TileShape);
    view.ToolsPanel.TilesPanel.SetWildcardIdText(model.WildcardId?.ToString() ?? "");

    uiSettings.Refresh();
  }

  private void OnGridColorChanged(Color color)
  {
    settings.EditorSettings.GridColor = color;
    redrawer.RedrawGrid();
  }

  private void OnSelectionColorChanged(Color color)
    => settings.EditorSettings.SelectionColor = color;

  private void OnGuiColorChanged(Color color)
  {
    settings.UiSettings.GuiColor = color;
    settings.UiSettings.Refresh();
  }

  private void OnBackgroundColorChanged(Color color)
  {
    settings.UiSettings.BackgroundColor = color;
    settings.UiSettings.Refresh();
  }

  private void OnProbabilityColorChanged(Color color)
  {
    settings.EditorSettings.ProbabilityColor = color;
    view.Workspace.TileProbability.UpdateFontColor(color);
  }

  private void OnTileSizeChanged(string text)
  {
    if (!int.TryParse(text, out var size) || size <= 0)
      return;

    model.TileSize = size;
    RedrawTiles();
  }

  private void OnFontSizeChanged(string text)
  {
    if (!int.TryParse(text, out var fontSize) || fontSize <= 0)
      return;

    settings.UiSettings.FontSize = fontSize;
    settings.UiSettings.Refresh();
  }

  private void OnResolutionChanged(Vector2I size, bool native)
  {
    var options = settings.DisplayOptions;
    options.Native = native;
    options.Width = size.X;
    options.Height = size.Y;
    ApplyDisplayOptions(options);
  }

  private void OnTileShapeChanged(TileShape tileShape)
  {
    model.TileShape = tileShape;
    RedrawTiles();
  }

  private void OnProjectRootChanged(string projectRoot)
  {
    model.ImageRootPath = projectRoot;
    view.ToolsPanel.SettingsPanel.SetProjectRootText(projectRoot);
  }

  private void RedrawTiles()
  {
    redrawer.RedrawAll();
    context.RaiseTileGeometryChanged();
  }

  private static void ApplyDisplayOptions(DisplayOptions options)
  {
    var resolution = options.Native
      ? DisplayServer.ScreenGetSize(DisplayServer.WindowGetCurrentScreen())
      : new Vector2I(options.Width, options.Height);

    SetContentScaleSize(resolution);
    DisplayServer.WindowSetSize(resolution);
  }

  private static void SetContentScaleSize(Vector2I size)
  {
    if (Engine.GetMainLoop() is SceneTree tree)
      tree.Root.ContentScaleSize = size;
  }
}
