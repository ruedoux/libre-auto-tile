using Godot;
using System;
using System.Collections.Generic;

namespace Qwaitumin.LibreAutoTile.GUI.Scenes.Utils;

public partial class DrawNode : Node2D
{
  private readonly Queue<Action> drawQueue = new();

  public DrawNode()
  {
    ZIndex = (int)RenderingServer.CanvasItemZMax;
  }

  public void DrawRectangle(
    Rect2I rectangle, Color color, bool filled = false, int width = 1)
  {
    // Yes this is stupid but godot throws warnings when you pass ANY width when filled is true...
    if (!filled)
      drawQueue.Enqueue(() => DrawRect(rectangle, color, filled: filled, width: width));
    if (filled)
      drawQueue.Enqueue(() => DrawRect(rectangle, color, filled: filled));
  }

  public void DrawSimpleLine(
    Vector2 from, Vector2 to, Color color, int width = 1, bool antialiasing = false)
      => drawQueue.Enqueue(() => DrawLine(from, to, color, width, antialiasing));

  public override void _Draw()
  {
    while (drawQueue.Count > 0)
      drawQueue.Dequeue()();
  }
}
