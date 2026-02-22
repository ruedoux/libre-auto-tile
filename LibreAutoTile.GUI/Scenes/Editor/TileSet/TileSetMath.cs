using Godot;
using Qwaitumin.LibreAutoTile.GUI.GodotBindings;
using Qwaitumin.LibreAutoTile.Tiling;

namespace Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.TileSet;

public static class TileSetMath
{
  public static readonly Vector2I TOP_LEFT = new(0, 0);
  public static readonly Vector2I TOP = new(1, 0);
  public static readonly Vector2I TOP_RIGHT = new(2, 0);
  public static readonly Vector2I LEFT = new(0, 1);
  public static readonly Vector2I RIGHT = new(2, 1);
  public static readonly Vector2I BOTTOM_LEFT = new(0, 2);
  public static readonly Vector2I BOTTOM = new(1, 2);
  public static readonly Vector2I BOTTOM_RIGHT = new(2, 2);
  public static readonly Vector2I MIDDLE = new(1, 1);

  public static readonly Vector2I[] BITMASK_POSITIONS =
  [
    TOP_LEFT, TOP, TOP_RIGHT, RIGHT, BOTTOM_RIGHT, BOTTOM, BOTTOM_LEFT, LEFT , MIDDLE
  ];

  public static TileMask.SurroundingDirection PositionToDirection(Vector2I position)
  {
    int index = Array.FindIndex(BITMASK_POSITIONS, x => x == position);
    if (index <= -1 || index >= 8)
      GodotLogger.LogErrorAndThrow($"Position cannot be mapped to tileMask: {position}");
    return (TileMask.SurroundingDirection)index;
  }

  public static Vector2I DirectionToPosition(TileMask.SurroundingDirection direction)
    => BITMASK_POSITIONS[(int)direction];

  public static Rect2I SnappedBitmaskPositionToWorldRectangle(
    Vector2I snappedTilePosition, Vector2I bitmaskPosition, int tileSize)
  {
    int segmentSize = tileSize / 3;
    int remainder = tileSize % 3;

    Vector2I worldBitmaskPosition = snappedTilePosition + bitmaskPosition * segmentSize;
    int width = segmentSize + (bitmaskPosition.X == 2 ? remainder : 0);
    int height = segmentSize + (bitmaskPosition.Y == 2 ? remainder : 0);
    return new Rect2I(worldBitmaskPosition, new Vector2I(width, height));
  }

  public static Vector2I DetermineBitmaskPosition(Vector2 worldPosition, int tileSize)
  {
    var snappedTilePosition = SnapToTileCorner(worldPosition, tileSize);

    int distanceX = Math.Abs((int)worldPosition.X - snappedTilePosition.X);
    int distanceY = Math.Abs((int)worldPosition.Y - snappedTilePosition.Y);
    Vector2I pointInTile = new(distanceX, distanceY);

    int segmentSize = tileSize / 3;
    int remainder = tileSize % 3;
    for (int index = 0; index < BITMASK_POSITIONS.Length; index++)
    {
      Vector2I position = BITMASK_POSITIONS[index];
      var x = position.X;
      var y = position.Y;
      int width = segmentSize + (x == 2 ? remainder : 0);
      int height = segmentSize + (y == 2 ? remainder : 0);
      var rectangle = new Rect2I(new(x * segmentSize, y * segmentSize), new(width, height));
      if (rectangle.HasPoint(pointInTile))
        return BITMASK_POSITIONS[index];
    }

    return Vector2I.Zero;
  }

  public static Vector2I SnapToTileCorner(Vector2 worldPosition, int tileSize)
    => ScaleDownTilePosition(worldPosition, tileSize) * tileSize;

  public static Vector2I ScaleDownTilePosition(Vector2 worldPosition, int tileSize)
  {
    int tileXScaledDown = (int)Math.Floor(worldPosition.X / tileSize);
    int tileYScaledDown = (int)Math.Floor(worldPosition.Y / tileSize);
    return new Vector2I(tileXScaledDown, tileYScaledDown);
  }
}
