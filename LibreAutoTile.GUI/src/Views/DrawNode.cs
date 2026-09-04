using Godot;

namespace Qwaitumin.LibreAutoTile.GUI.Views;

public partial class DrawNode : Node2D
{
  private readonly Queue<Action> drawQueue = new();

  public void DrawRectangle(
    Rect2I rectangle, Color color, bool filled = false, int width = 1)
  {
    // Yes this is stupid but godot throws warnings when you pass ANY width when filled is true...
    if (filled)
      drawQueue.Enqueue(() => DrawRect(rectangle, color, filled: filled));
    else
      drawQueue.Enqueue(() => DrawRect(rectangle, color, filled: filled, width: width));
  }

  public void DrawSimpleLine(
    Vector2 from, Vector2 to, Color color, int width = 1, bool antialiasing = false)
      => drawQueue.Enqueue(() => DrawLine(from, to, color, width, antialiasing));

  public void DrawPolygon(
    Vector2[] points, Color color, bool filled = false, int width = 1)
  {
    if (filled)
      drawQueue.Enqueue(() => DrawColoredPolygon(points, color));
    else
      drawQueue.Enqueue(() => DrawPolyline([.. points, points[0]], color, width));
  }

  public void Clear()
    => drawQueue.Clear();

  public override void _Draw()
  {
    while (drawQueue.Count > 0)
      drawQueue.Dequeue()();
  }
}
