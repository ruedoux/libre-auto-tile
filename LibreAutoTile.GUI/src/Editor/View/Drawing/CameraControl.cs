using Godot;
using LibreAutoTile.GUI.Settings;

namespace LibreAutoTile.GUI.Editor.View.Drawing;

public partial class CameraControl : Camera2D
{
  public Rect2I View = new();

  public float MaxZoom;
  public float MinZoom;
  public float ZoomValue;

  public CameraControl(AppSettings settings)
  {
    MaxZoom = settings.CameraSettings.MaxZoom;
    MinZoom = settings.CameraSettings.MinZoom;
    ZoomValue = settings.CameraSettings.ZoomStep;
  }

  public void MoveCamera(Vector2 direction)
  {
    var positionToMove = GlobalPosition + direction;

    if (View.Position.X > positionToMove.X || View.End.X < positionToMove.X)
      return;
    if (View.Position.Y > positionToMove.Y || View.End.Y < positionToMove.Y)
      return;

    GlobalPosition = positionToMove;
  }

  public void ZoomCamera(float value)
  {
    if (Zoom.X + value < MinZoom)
      return;
    if (Zoom.X + value > MaxZoom)
      return;
    Zoom = new Vector2(Zoom.X + value, Zoom.Y + value);
  }
}
