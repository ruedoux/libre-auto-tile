using Godot;
using LibreAutoTile.Configuration;
using LibreAutoTile.GUI.Editor.Model;
using LibreAutoTile.GUI.Editor.View;
using LibreAutoTile.GUI.Settings;

namespace LibreAutoTile.GUI.Editor.Controller;

public class ConfigurationController
{
  private readonly EditorContext context;
  private readonly EditorModel model;
  private readonly EditorView view;
  private readonly AppSettings settings;
  private readonly GuiLogger logger;
  private readonly EditorRedrawer redrawer;

  public ConfigurationController(EditorContext context)
  {
    this.context = context;
    model = context.Model;
    view = context.View;
    settings = context.Settings;
    logger = context.Logger;
    redrawer = context.Redrawer;

    view.Toolbar.SelectImageButton.Pressed += ShowImageDialog;
    view.Toolbar.SelectImageDialog.FileSelected += LoadImageFromFile;
    view.Toolbar.SaveButton.Pressed += ShowSaveConfigurationDialog;
    view.Toolbar.SaveConfigurationDialog.FileSelected += SaveConfiguration;
    view.Toolbar.LoadButton.Pressed += ShowLoadConfigurationDialog;
    view.Toolbar.LoadConfigurationDialog.FileSelected += LoadConfiguration;
    view.Toolbar.ClearButton.Pressed += ClearConfiguration;
    view.ToolsPanel.TilesPanel.ImageSelected += OnImageSelected;
  }

  private void ShowImageDialog()
    => ShowDialog(view.Toolbar.SelectImageDialog);

  private void ShowSaveConfigurationDialog()
    => ShowDialog(view.Toolbar.SaveConfigurationDialog);

  private void ShowLoadConfigurationDialog()
    => ShowDialog(view.Toolbar.LoadConfigurationDialog);

  private void ShowDialog(FileDialog dialog)
  {
    if (!string.IsNullOrEmpty(model.ImageRootPath))
      dialog.CurrentDir = model.ImageRootPath;
    dialog.PopupCenteredRatio(settings.UiSettings.DialogScreenRatio);
  }

  private void SaveConfiguration(string filePath)
  {
    AutoTileConfiguration configuration = AutoTileConfigurationConverter.GetAsAutoTileConfiguration(
      model.Tiles.Tiles,
      model.BitmaskDatabase,
      model.ImageRootPath,
      model.TileSize,
      model.TileShape,
      model.WildcardId,
      model.ImageRootPath);
    var jsonString = configuration.ToJsonString();
    File.WriteAllText(filePath, jsonString);
    logger.Log($"Saved to: {filePath}");
    view.MessageDisplay.DisplayText($"[color=green]Saved to: {filePath}[/color]");
  }

  private void LoadConfiguration(string filePath)
  {
    AutoTileConfiguration autoTileConfiguration;
    try
    {
      autoTileConfiguration = AutoTileConfigurationConverter.LoadConfiguration(
        filePath, model.Tiles, model.BitmaskDatabase, model.ImageRootPath);
    }
    catch (Exception ex)
    {
      logger.LogException($"Failed to load configuration from '{filePath}'", ex);
      throw;
    }

    model.TileSize = (int)autoTileConfiguration.TileSize;
    model.TileShape = autoTileConfiguration.TileShape;
    view.ToolsPanel.SettingsPanel.SetTileSizeText(model.TileSize.ToString());
    view.ToolsPanel.SettingsPanel.SelectTileShape(model.TileShape);

    model.WildcardId = (int?)autoTileConfiguration.WildcardId;
    view.ToolsPanel.TilesPanel.SetWildcardIdText(model.WildcardId?.ToString() ?? "");

    model.ImagePath = "";
    model.ImageSize = Vector2I.Zero;
    view.Workspace.ClearImage();

    redrawer.RefreshTilesView();
    redrawer.RedrawAll();
    context.RaiseDataChanged();

    string? firstImage = model.BitmaskDatabase.GetAll().Keys.FirstOrDefault();
    if (firstImage is not null)
    {
      if (File.Exists(firstImage))
        LoadImageFromPath(firstImage);
      else
        view.MessageDisplay.DisplayText(
          $"[color=yellow]Could not find image referenced by configuration: {firstImage}[/color]");
    }

    redrawer.RefreshImageOptions();
    logger.Log($"Loaded from: {filePath}");
    view.MessageDisplay.DisplayText($"[color=green]Loaded from: {filePath}[/color]");
  }

  private void ClearConfiguration()
  {
    logger.Log("> Starting clearing editor state");
    model.Tiles.Clear();
    model.BitmaskDatabase.Clear();
    redrawer.RefreshTilesView();
    redrawer.RedrawBitmask();
    redrawer.RedrawProbabilityLabels();
    context.RaiseDataChanged();
    redrawer.RefreshImageOptions();
    logger.Log("> Finished clearing editor state");
    view.MessageDisplay.DisplayText("[color=green]Cleared configuration[/color]");
  }

  private void LoadImageFromFile(string path)
    => LoadImageFromPath(Path.GetFullPath(path));

  private void LoadImageFromPath(string imagePath)
  {
    var image = Image.LoadFromFile(imagePath);
    image.Resize(
      image.GetWidth() * settings.WorldSettings.ImageScaling,
      image.GetHeight() * settings.WorldSettings.ImageScaling,
      Image.Interpolation.Nearest);

    var texture = ImageTexture.CreateFromImage(image);
    model.ImagePath = imagePath;
    model.ImageSize = new Vector2I(image.GetWidth(), image.GetHeight());

    view.Workspace.SetImage(texture);
    view.Workspace.SetCameraView(new(Vector2I.Zero, model.ImageSize));
    redrawer.RedrawAll();
    context.RaiseDataChanged();
    logger.Log($"Changed image to: {imagePath}");
    redrawer.RefreshImageOptions();
  }

  private void OnImageSelected(string imageName)
  {
    if (!File.Exists(imageName))
    {
      view.MessageDisplay.DisplayText($"[color=yellow]Could not find image: {imageName}[/color]");
      redrawer.RefreshImageOptions();
      return;
    }

    LoadImageFromPath(imageName);
  }
}
