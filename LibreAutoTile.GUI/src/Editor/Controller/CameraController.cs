using Godot;
using LibreAutoTile.GUI.Editor.View;

namespace LibreAutoTile.GUI.Editor.Controller;

public class CameraController(EditorContext context)
{
  private readonly EditorView view = context.View;

  public void Pan(Vector2 relative)
    => view.Workspace.CameraControl.MoveCamera(-relative);

  public void Zoom(int delta)
    => view.Workspace.CameraControl.ZoomCamera(delta * view.Workspace.CameraControl.ZoomValue);
}
