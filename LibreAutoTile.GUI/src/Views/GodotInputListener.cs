using Godot;

namespace Qwaitumin.LibreAutoTile.GUI.Views;

public class GodotInputListener
{
  public bool Active = true;

  private readonly List<Action<InputEvent>> inputActions = [];
  private readonly List<Action<InputEventMouse>> inputMouseActions = [];
  private readonly List<Action<InputEventMouseButton>> inputMouseButtonActions = [];
  private readonly List<Action<InputEventMouseMotion>> inputMouseMotionActions = [];

  public void AddInputAction(Action<InputEvent> action)
    => inputActions.Add(action);

  public void AddInputMouseAction(Action<InputEventMouse> action)
    => inputMouseActions.Add(action);

  public void AddInputMouseButtonAction(Action<InputEventMouseButton> action)
    => inputMouseButtonActions.Add(action);

  public void AddInputMouseMotionAction(Action<InputEventMouseMotion> action)
    => inputMouseMotionActions.Add(action);

  public void ListenToInput(InputEvent inputEvent)
  {
    if (!Active)
      return;

    foreach (var action in inputActions)
      action(inputEvent);

    foreach (var action in inputMouseActions)
      if (inputEvent is InputEventMouse inputEventType)
        action(inputEventType);

    foreach (var action in inputMouseButtonActions)
      if (inputEvent is InputEventMouseButton inputEventType)
        action(inputEventType);

    foreach (var action in inputMouseMotionActions)
      if (inputEvent is InputEventMouseMotion inputEventType)
        action(inputEventType);
  }
}
