using System.Numerics;
using Qwaitumin.AutoTile;

namespace Qwaitumin.AutoTileTests;


class MockedTileMapDrawer : ITileMapDrawer
{
  public Dictionary<Vector2, TileData>[] Data;

  public MockedTileMapDrawer(int layerCount)
  {
    Data = new Dictionary<Vector2, TileData>[layerCount];
    for (int layer = 0; layer < layerCount; layer++)
      Data[layer] = new();
  }

  public void Clear()
  {
    foreach (var dataLayer in Data)
      dataLayer.Clear();
  }

  public void ClearTiles(int layer, Vector2[] positions)
  {
    foreach (var position in positions)
      Data[layer].Remove(position);
  }

  public void DrawTiles(int tileLayer, KeyValuePair<Vector2, TileData?>[] positionsToTileData)
  {
    foreach (var (position, tileData) in positionsToTileData)
      if (tileData is not null)
        Data[tileLayer][position] = tileData;
  }
}