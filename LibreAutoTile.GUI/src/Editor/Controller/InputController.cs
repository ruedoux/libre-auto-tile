using Godot;
using LibreAutoTile.GUI.Editor.Controller.Tools;
using LibreAutoTile.GUI.Editor.View;

namespace LibreAutoTile.GUI.Editor.Controller;

public class InputController
{
  private readonly EditorView view;
  private readonly ToolStateMachine toolStateMachine;
  private readonly CameraController camera;

  public InputController(EditorContext context, ToolStateMachine toolStateMachine)
  {
    view = context.View;
    this.toolStateMachine = toolStateMachine;
    camera = new CameraController(context);

    view.MouseMotion += OnMouseMotion;
    view.MouseButton += OnMouseButton;

    view.MouseClick += toolStateMachine.HandleMouseClick;
  }

  private void OnMouseMotion(InputEventMouseMotion mouseMotion)
  {
    bool isMouseOnUi = GodotExtensions.IsMouseOnElements(view.UiElements);
    view.Workspace.SetMouseLabelVisible(!isMouseOnUi && toolStateMachine.ShowsMouseLabel);
    if (isMouseOnUi)
      view.Workspace.BitmaskDrawer.HideBitmaskGhost();
    else
      view.Workspace.BitmaskDrawer.ShowBitmaskGhost();

    if (mouseMotion.ButtonMask == MouseButtonMask.Middle)
      camera.Pan(mouseMotion.Relative);
    else if (!isMouseOnUi)
      toolStateMachine.HandleMouseMotion(view.GetGlobalMousePosition());
  }

  private void OnMouseButton(InputEventMouseButton mouseButton)
  {
    int delta;
    if (mouseButton.ButtonIndex == MouseButton.WheelDown)
      delta = -1;
    else if (mouseButton.ButtonIndex == MouseButton.WheelUp)
      delta = 1;
    else
      return;

    bool isMouseOnUi = GodotExtensions.IsMouseOnElements(view.UiElements);
    if (!toolStateMachine.HandleMouseWheel(delta, mouseButton.ShiftPressed, isMouseOnUi) && !isMouseOnUi)
      camera.Zoom(delta);
  }
}
