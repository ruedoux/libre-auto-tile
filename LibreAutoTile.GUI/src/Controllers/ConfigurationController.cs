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
    var view = context.View;
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
    => context.View.SelectImageDialog.PopupCenteredRatio(Settings.DIALOG_SCREEN_RATIO);

  private void ShowSaveConfigurationDialog()
    => context.View.SaveConfigurationDialog.PopupCenteredRatio(Settings.DIALOG_SCREEN_RATIO);

  private void ShowLoadConfigurationDialog()
    => context.View.LoadConfigurationDialog.PopupCenteredRatio(Settings.DIALOG_SCREEN_RATIO);

  private void SaveConfiguration(string filePath)
  {
    AutoTileConfiguration configuration = AutoTileConfigurationConverter.GetAsAutoTileConfiguration(
      context.Data.Tiles.Tiles, context.Data.BitmaskDatabase, context.Data.TileSize, context.Data.TileShape);
    var jsonString = configuration.ToJsonString();
    File.WriteAllText(filePath, jsonString);
    GodotLogger.LOGGER.Log($"Saved to: {filePath}");
    context.View.MessageDisplay.DisplayText($"[color=green]Saved to: {filePath}[/color]");
  }

  private void LoadConfiguration(string filePath)
  {
    AutoTileConfiguration autoTileConfiguration = AutoTileConfigurationConverter.LoadConfiguration(
      filePath, context.Data.Tiles, context.Data.BitmaskDatabase);

    context.Data.TileSize = (int)autoTileConfiguration.TileSize;
    context.Data.TileShape = autoTileConfiguration.TileShape;
    context.View.SettingsPanel.SetTileSizeText(context.Data.TileSize.ToString());
    context.View.SettingsPanel.SelectTileShape(context.Data.TileShape);

    context.Data.ImagePath = "";
    context.Data.ImageSize = Vector2I.Zero;
    context.View.ClearImage();

    context.RefreshTilesView();
    context.RedrawGrid();
    context.RedrawBitmask();
    context.RedrawProbabilityLabels();
    context.Probability.ResetSelection();

    string? firstImage = context.Data.BitmaskDatabase.GetAll().Keys.FirstOrDefault();
    if (firstImage is not null)
    {
      if (File.Exists(firstImage))
        LoadImageFromPath(firstImage);
      else
        context.View.MessageDisplay.DisplayText(
          $"[color=yellow]Could not find image referenced by configuration: {firstImage}[/color]");
    }

    context.RefreshImageOptions();
    GodotLogger.LOGGER.Log($"Loaded from: {filePath}");
    context.View.MessageDisplay.DisplayText($"[color=green]Loaded from: {filePath}[/color]");
  }

  private void ClearConfiguration()
  {
    GodotLogger.LOGGER.Log("> Starting clearing editor state");
    context.Data.Tiles.Clear();
    context.Data.BitmaskDatabase.Clear();
    context.RefreshTilesView();
    context.RedrawBitmask();
    context.RedrawProbabilityLabels();
    context.Probability.ResetSelection();
    context.RefreshImageOptions();
    GodotLogger.LOGGER.Log("> Finished clearing editor state");
    context.View.MessageDisplay.DisplayText("[color=green]Cleared configuration[/color]");
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
    context.Data.ImagePath = relativePath;
    context.Data.ImageSize = new Vector2I(image.GetWidth(), image.GetHeight());

    context.View.SetImage(texture);
    context.View.SetCameraView(new(Vector2I.Zero, context.Data.ImageSize));
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
      context.View.MessageDisplay.DisplayText($"[color=yellow]Could not find image: {imageName}[/color]");
      context.RefreshImageOptions();
      return;
    }

    LoadImageFromPath(imageName);
  }
}
