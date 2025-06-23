using Qwaitumin.LibreAutoTile.Configuration.Models;
using Qwaitumin.LibreAutoTile.Tiling;

namespace Qwaitumin.LibreAutoTile.Tests;

class MockedTileMapDrawer : ITileMapDrawer
{
  public Dictionary<Vector2, TileData>[] Data;

  public MockedTileMapDrawer(int layerCount)
  {
    Data = new Dictionary<Vector2, TileData>[layerCount];
    for (int layer = 0; layer < layerCount; layer++)
      Data[layer] = [];
  }

  public void Clear()
  {
    foreach (var dataLayer in Data)
      dataLayer.Clear();
  }

  public void ClearTiles(int layer, IEnumerable<Vector2> positions)
  {
    foreach (var position in positions)
      Data[layer].Remove(position);
  }

  public void DrawTiles(int tileLayer, IEnumerable<(Vector2 Position, TileData TileData)> positionsToTileData)
  {
    foreach (var (position, tileData) in positionsToTileData)
      Data[tileLayer][position] = tileData;
  }
}