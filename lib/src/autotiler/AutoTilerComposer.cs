namespace Qwaitumin.AutoTile;

public class AutoTilerComposer
{
  public readonly TileLoader TileLoader;
  private readonly AutoTileConfig autoTileConfig;

  public AutoTilerComposer(
    string imageDirectoryPath,
    AutoTileConfig autoTileConfig,
    string[] tileIdToTileNames)
  {
    this.autoTileConfig = autoTileConfig;
    TileLoader = new(imageDirectoryPath, autoTileConfig, tileIdToTileNames);
  }

  public AutoTiler GetAutoTiler()
  {
    var biggestLayer = autoTileConfig.TileDefinitions
      .DistinctBy(e => e.Value.Layer)
      .OrderByDescending(e => e.Value.Layer)
      .FirstOrDefault().Value.Layer;

    return new(biggestLayer + 1, GetAutoTileData(TileLoader.LoadTiles()));
  }

  private static AutoTileData[] GetAutoTileData(Dictionary<TileIdentificator, TileResource> tiles)
  {
    var autoTileIdToTileDatas = new AutoTileData[tiles.Count];
    Dictionary<int, bool[]> autoTileGroupToConnections = new();

    foreach (var (_, tileResource) in tiles)
      autoTileGroupToConnections[tileResource.AutoTileGroup] = new bool[tiles.Count];

    foreach (var (tileIdentificator, tileResource) in tiles)
      autoTileGroupToConnections[tileResource.AutoTileGroup][tileIdentificator.TileId] = true;

    foreach (var (tileIdentificator, tileResource) in tiles)
      autoTileIdToTileDatas[tileIdentificator.TileId] = new(
        autoTileGroupToConnections[tileResource.AutoTileGroup],
        tiles[tileIdentificator].BitmaskSet);

    return autoTileIdToTileDatas;
  }
}