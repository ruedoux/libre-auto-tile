using System.Numerics;

namespace Qwaitumin.AutoTile;

// NOTE: This has to work as fast as possible
public class AutoTileData
{
  private static Vector2 DEFAULT_ATLAS_POSITION = Vector2.Zero;

  private readonly bool[] tileIdConnections;
  private readonly Vector2[] computedBitmaskToAtlasPosition = new Vector2[byte.MaxValue + 1];

  public AutoTileData(
    bool[] tileIdConnections,
    Dictionary<byte, Vector2> bitmaskToAtlasPositions)
  {
    this.tileIdConnections = tileIdConnections;
    for (int i = 0; i < computedBitmaskToAtlasPosition.Length; i++)
      computedBitmaskToAtlasPosition[i] = DEFAULT_ATLAS_POSITION;
    foreach (var (bitmask, atlasPosition) in bitmaskToAtlasPositions)
      computedBitmaskToAtlasPosition[bitmask] = atlasPosition;
  }

  public bool CanConnectTo(int tileId)
    => tileIdConnections[tileId];

  public Vector2 GetAtlasCoords(byte computedBitmask)
    => computedBitmaskToAtlasPosition[computedBitmask];
}