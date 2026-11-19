using Godot;
using LibreAutoTile.GUI.Editor.Controller;
using LibreAutoTile.GUI.Editor.Model;
using LibreAutoTile.GUI.Editor.View;
using LibreAutoTile.GUI.Settings;

namespace LibreAutoTile.GUI;

public partial class EditorScene : Control
{
  private readonly EditorModel model;
  private readonly EditorView view;
  private readonly GuiLogger logger;

  public EditorScene()
  {
    var settings = new AppSettings();
    if (OS.IsDebugBuild())
      settings.LogFilePath = "./guilog.txt";

    var windowSize = DisplayServer.WindowGetSize();
    settings.DisplayOptions = DisplayOptions.CreateDefault(windowSize.X, windowSize.Y);

    logger = new GuiLogger(GD.PrintRich, settings.LogFilePath);
    model = new EditorModel();
    view = this.AppendChild(new EditorView(settings, logger));
    _ = new EditorController(model, view, settings, logger);
  }
}
