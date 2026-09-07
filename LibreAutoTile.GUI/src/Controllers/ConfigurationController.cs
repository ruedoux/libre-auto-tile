using Godot;
using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.GUI.Models;
using Qwaitumin.LibreAutoTile.GUI.Views;

namespace Qwaitumin.LibreAutoTile.GUI.Controllers;

public class ConfigurationController
{
  private readonly EditorContext context;

  public ConfigurationController(EditorContext context)
  {
    this.context = context;
    var view = context.EditorScene;
    view.SelectImageButton.Pressed += ShowImageDialog;
    view.SelectImageDialog.FileSelected += LoadImageFromFile;
    view.SaveButton.Pressed += ShowSaveConfigurationDialog;
    view.SaveConfigurationDialog.FileSelected += SaveConfiguration;
    view.LoadButton.Pressed += ShowLoadConfigurationDialog;
    view.LoadConfigurationDialog.FileSelected += LoadConfiguration;
    view.ClearButton.Pressed += ClearConfiguration;
    view.TilesPanel.ImageSelected += OnImageSelected;
  }

  private void ShowImageDialog()
    => context.EditorScene.SelectImageDialog.PopupCenteredRatio(Settings.DIALOG_SCREEN_RATIO);

  private void ShowSaveConfigurationDialog()
    => context.EditorScene.SaveConfigurationDialog.PopupCenteredRatio(Settings.DIALOG_SCREEN_RATIO);

  private void ShowLoadConfigurationDialog()
    => context.EditorScene.LoadConfigurationDialog.PopupCenteredRatio(Settings.DIALOG_SCREEN_RATIO);

  private void SaveConfiguration(string filePath)
  {
    AutoTileConfiguration configuration = AutoTileConfigurationConverter.GetAsAutoTileConfiguration(
      context.EditorData.Tiles.Tiles, context.EditorData.BitmaskDatabase, context.EditorData.TileSize, context.EditorData.TileShape,
      context.EditorData.WildcardId);
    var jsonString = configuration.ToJsonString();
    File.WriteAllText(filePath, jsonString);
    GodotLogger.LOGGER.Log($"Saved to: {filePath}");
    context.EditorScene.MessageDisplay.DisplayText($"[color=green]Saved to: {filePath}[/color]");
  }

  private void LoadConfiguration(string filePath)
  {
    AutoTileConfiguration autoTileConfiguration = AutoTileConfigurationConverter.LoadConfiguration(
      filePath, context.EditorData.Tiles, context.EditorData.BitmaskDatabase);

    context.EditorData.TileSize = (int)autoTileConfiguration.TileSize;
    context.EditorData.TileShape = autoTileConfiguration.TileShape;
    context.EditorScene.SettingsPanel.SetTileSizeText(context.EditorData.TileSize.ToString());
    context.EditorScene.SettingsPanel.SelectTileShape(context.EditorData.TileShape);

    context.EditorData.WildcardId = (int?)autoTileConfiguration.WildcardId;
    context.EditorScene.TilesPanel.SetWildcardIdText(context.EditorData.WildcardId?.ToString() ?? "");

    context.EditorData.ImagePath = "";
    context.EditorData.ImageSize = Vector2I.Zero;
    context.EditorScene.ClearImage();

    context.RefreshTilesView();
    context.RedrawGrid();
    context.RedrawBitmask();
    context.RedrawProbabilityLabels();
    context.Probability.ResetSelection();

    string? firstImage = context.EditorData.BitmaskDatabase.GetAll().Keys.FirstOrDefault();
    if (firstImage is not null)
    {
      if (File.Exists(firstImage))
        LoadImageFromPath(firstImage);
      else
        context.EditorScene.MessageDisplay.DisplayText(
          $"[color=yellow]Could not find image referenced by configuration: {firstImage}[/color]");
    }

    context.RefreshImageOptions();
    GodotLogger.LOGGER.Log($"Loaded from: {filePath}");
    context.EditorScene.MessageDisplay.DisplayText($"[color=green]Loaded from: {filePath}[/color]");
  }

  private void ClearConfiguration()
  {
    GodotLogger.LOGGER.Log("> Starting clearing editor state");
    context.EditorData.Tiles.Clear();
    context.EditorData.BitmaskDatabase.Clear();
    context.RefreshTilesView();
    context.RedrawBitmask();
    context.RedrawProbabilityLabels();
    context.Probability.ResetSelection();
    context.RefreshImageOptions();
    GodotLogger.LOGGER.Log("> Finished clearing editor state");
    context.EditorScene.MessageDisplay.DisplayText("[color=green]Cleared configuration[/color]");
  }

  private void LoadImageFromFile(string path)
    => LoadImageFromPath(Path.GetRelativePath(".", path));

  private void LoadImageFromPath(string relativePath)
  {
    var image = Image.LoadFromFile(relativePath);
    image.Resize(
      image.GetWidth() * Settings.IMAGE_SCALING,
      image.GetHeight() * Settings.IMAGE_SCALING,
      Image.Interpolation.Nearest);

    var texture = ImageTexture.CreateFromImage(image);
    context.EditorData.ImagePath = relativePath;
    context.EditorData.ImageSize = new Vector2I(image.GetWidth(), image.GetHeight());

    context.EditorScene.SetImage(texture);
    context.EditorScene.SetCameraView(new(Vector2I.Zero, context.EditorData.ImageSize));
    context.RedrawGrid();
    context.RedrawProbabilityLabels();
    context.RedrawBitmask();
    context.Probability.ResetSelection();
    GodotLogger.LOGGER.Log($"Changed image to: {relativePath}");
    context.RefreshImageOptions();
  }

  private void OnImageSelected(string imageName)
  {
    if (!File.Exists(imageName))
    {
      context.EditorScene.MessageDisplay.DisplayText($"[color=yellow]Could not find image: {imageName}[/color]");
      context.RefreshImageOptions();
      return;
    }

    LoadImageFromPath(imageName);
  }
}
