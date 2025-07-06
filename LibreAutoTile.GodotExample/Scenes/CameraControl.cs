using Godot;

namespace Qwaitumin.LibreAutoTile.GodotExample.Scenes;

public partial class CameraControl : Camera2D
{
  public Rect2I View = new();

  public float MaxZoom = 8.0f;
  public float MinZoom = 0.5f;
  public int MoveSpeed = 5;
  public float ZoomValue = 0.05f;

  public override void _Input(InputEvent @event)
  {
    if (@event is InputEventMouseButton mouseButton)
    {
      if (MouseButton.WheelDown == mouseButton.ButtonIndex)
        ZoomCamera(-ZoomValue);
      if (MouseButton.WheelUp == mouseButton.ButtonIndex)
        ZoomCamera(ZoomValue);
    }

    if (@event is InputEventMouseMotion mouseMotion)
    {
      if (MouseButtonMask.Middle == mouseMotion.ButtonMask)
        MoveCamera(-mouseMotion.Relative);
    }
  }

  private void MoveCamera(Vector2 direction)
  {
    var positionToMove = GlobalPosition + direction;

    if (View.Position.X > positionToMove.X || View.End.X < positionToMove.X)
      return;
    if (View.Position.Y > positionToMove.Y || View.End.Y < positionToMove.Y)
      return;

    GlobalPosition = positionToMove;
  }

  private void ZoomCamera(float value)
  {
    if (Zoom.X + value < MinZoom)
      return;
    if (Zoom.X + value > MaxZoom)
      return;
    Zoom = new Vector2(Zoom.X + value, Zoom.Y + value);
  }
}

