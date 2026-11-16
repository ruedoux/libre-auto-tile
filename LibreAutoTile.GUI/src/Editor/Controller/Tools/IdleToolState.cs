using Godot;

namespace LibreAutoTile.GUI.Editor.Controller.Tools;

public class IdleToolState(EditorContext context) : IToolState
{
  public void HandleMouseMotion(Vector2 position)
    => context.Redrawer.DrawTileHover(position);
}
