using Godot;
using LibreAutoTile.GUI.Settings;

namespace LibreAutoTile.GUI.Editor.View;

public partial class Toolbar : MarginContainer
{
  public readonly Button SelectImageButton;
  public readonly Button SaveButton;
  public readonly Button LoadButton;
  public readonly Button ClearButton;

  public readonly FileDialog SelectImageDialog;
  public readonly FileDialog SaveConfigurationDialog;
  public readonly FileDialog LoadConfigurationDialog;

  public Toolbar(AppSettings settings)
  {
    SelectImageDialog = this.AppendFileDialog("Select Image")
      .WithFileMode(FileDialog.FileModeEnum.OpenFile)
      .WithFilters(["*.jpg", "*.png"]);

    SaveConfigurationDialog = this.AppendFileDialog("Save Configuration")
      .WithFileMode(FileDialog.FileModeEnum.SaveFile)
      .WithFilters(["*.json"]);

    LoadConfigurationDialog = this.AppendFileDialog("Load Configuration")
      .WithFileMode(FileDialog.FileModeEnum.OpenFile)
      .WithFilters(["*.json"]);

    this.ExpandHorizontal()
      .WithMargins(0, settings.UiSettings.MarginBig, settings.UiSettings.MarginMedium, settings.UiSettings.MarginMedium);

    var hbox = this.AppendHBox().ExpandHorizontal();
    SelectImageButton = hbox.AppendButton("Select Image").ExpandHorizontal();
    SaveButton = hbox.AppendButton("Save").ExpandHorizontal();
    LoadButton = hbox.AppendButton("Load").ExpandHorizontal();
    ClearButton = hbox.AppendButton("Clear").ExpandHorizontal();
  }
}
