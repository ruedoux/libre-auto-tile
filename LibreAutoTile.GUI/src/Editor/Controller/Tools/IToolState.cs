using Godot;

namespace LibreAutoTile.GUI.Editor.Controller.Tools;

public interface IToolState
{
  bool CanActivate() => true;
  string ActivationError => string.Empty;
  bool ShowsMouseLabel => true;

  void Enter() { }
  void Exit() { }

  void HandleMouseInput(InputEventMouse mouse, Vector2 position) { }
  void HandleMouseMotion(Vector2 position) { }
  bool HandleMouseWheel(int delta, bool shiftPressed) => false;
}
