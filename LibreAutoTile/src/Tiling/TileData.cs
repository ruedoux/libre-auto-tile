using Qwaitumin.LibreAutoTile.Tiling.Search.Models;

namespace Qwaitumin.LibreAutoTile.Tiling;

public struct TileData
{
  public int CentreTileId { get; set; } = -1;
  public TileMask TileMask { get; set; } = new();
  public TileAtlas TileAtlas { get; set; } = new();

  public static TileData Empty => new() { CentreTileId = -1 };

  public TileData() { }

  public TileData(int centreTileId, TileMask tileMask, TileAtlas tileAtlas) : this()
  {
    CentreTileId = centreTileId;
    TileMask = tileMask;
    TileAtlas = tileAtlas;
  }

  public readonly bool IsEmpty() => CentreTileId == -1;

  public override readonly string ToString()
    => $"\"CentreTileId\":{CentreTileId}, \"TileMask\":{TileMask}, \"TileAtlas\":{TileAtlas}";
}