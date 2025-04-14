using System;
using Godot;

namespace Qwaitumin.AutoTile.GUI.Scenes.Editor.TileSet;

public static class BitmaskCalculator
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
    TOP_LEFT, TOP, TOP_RIGHT, LEFT, RIGHT, BOTTOM_LEFT, BOTTOM, BOTTOM_RIGHT, MIDDLE
  ];

  public static TileMask.SurroundingDirection PositionToDirection(Vector2I position)
  {
    int index = Array.FindIndex(BITMASK_POSITIONS, x => x == position);
    if (index == -1)
      throw new ArgumentException($"Position cannot be mapped to tileMask: {position}");
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

  public static Vector2I DetermineBitmask(Vector2 worldPosition, int tileSize)
  {
    var snappedTilePosition = TileSetMath.SnapToTileCorner(worldPosition, tileSize);

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
}
