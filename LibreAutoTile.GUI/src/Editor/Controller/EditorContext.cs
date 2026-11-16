using LibreAutoTile.GUI.Editor.Model;
using LibreAutoTile.GUI.Editor.View;
using LibreAutoTile.GUI.Settings;

namespace LibreAutoTile.GUI.Editor.Controller;

public class EditorContext(
  EditorModel model,
  EditorView view,
  AppSettings settings,
  GuiLogger logger,
  EditorRedrawer redrawer)
{
  public EditorModel Model { get; } = model;
  public EditorView View { get; } = view;
  public AppSettings Settings { get; } = settings;
  public GuiLogger Logger { get; } = logger;
  public EditorRedrawer Redrawer { get; } = redrawer;

  public event Action? DataChanged;
  public event Action? TileGeometryChanged;

  public void RaiseDataChanged()
    => DataChanged?.Invoke();

  public void RaiseTileGeometryChanged()
    => TileGeometryChanged?.Invoke();
}
