using Godot;
using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.Tiling;

namespace Qwaitumin.LibreAutoTile.GodotBindings;

public class AutoTileMap : Node2D
{
  private readonly TileMapWrapper[] tileMapWrappers;
  private readonly AutoTileDrawer autoTileDrawer;
  private readonly TileMapDrawer tileMapDrawer;

  /// <summary>
  /// Constructs the tile map. Must be called on the main thread: this creates Godot
  /// Resources and Nodes (image loading, TileSet/atlas sources, CreateTile, AddChild)
  /// that are not safe to build from a background thread
  /// </summary>
  public AutoTileMap(
    int layerCount,
    AutoTileConfiguration autoTileConfiguration,
    TileSet.TileShapeEnum tileShape = TileSet.TileShapeEnum.Square)
  {
    foreach (var (_, tileDefinition) in autoTileConfiguration.TileDefinitions)
      foreach (var (imageFileName, _) in tileDefinition.ImageFileNameToTileMaskDefinition)
        if (!File.Exists(imageFileName))
          throw new FileNotFoundException($"File defined in configuration does not exist: '{imageFileName}'");

    tileMapWrappers = new TileMapWrapper[layerCount];
    for (int layer = 0; layer < layerCount; layer++)
    {
      TileMapWrapper tileMapWrapper = new(autoTileConfiguration, tileShape);
      tileMapWrappers[layer] = tileMapWrapper;
      AddChild(tileMapWrapper.TileMapLayer);
    }

    tileMapDrawer = new TileMapDrawer(tileMapWrappers);
    autoTileDrawer = new(
      tileMapDrawer, new AutoTiler(layerCount, AutoTileConfigurationExtractor.BuildTileIdToTileMaskSearcher(autoTileConfiguration)));
  }

  public Vector2I WorldToMap(Vector2 localPosition)
    => tileMapWrappers[0].TileMapLayer.LocalToMap(localPosition);

  public int GetSourceId(string imageFileName)
    => tileMapDrawer.GetSourceId(imageFileName);

  public Tiling.TileData GetTile(int layer, Vector2I position)
    => autoTileDrawer.GetTile(layer, GodotTypeMapper.Map(position));

  public void Clear()
    => autoTileDrawer.Clear();

  public int GetLayerCount()
    => tileMapWrappers.Length;

  public TileMapLayer GetTileMapLayer(int layer)
  {
    if (tileMapWrappers.Length <= layer)
      throw new IndexOutOfRangeException($"Layer does not exist: {layer}");
    return tileMapWrappers[layer].TileMapLayer;
  }

  public async Task DrawTilesAsync(int layer, IEnumerable<(Vector2I Position, int TileId)> positionToTileIds)
    => await Task.Run(() => DrawTiles(layer, positionToTileIds));

  public void DrawTiles(int layer, IEnumerable<(Vector2I Position, int TileId)> positionToTileIds)
  {
    var positionToTileIdsConverted = positionToTileIds
      .Select(positionToTileId => (GodotTypeMapper.Map(positionToTileId.Position), positionToTileId.TileId))
      .ToArray();
    autoTileDrawer.DrawTiles(layer, positionToTileIdsConverted);
  }

  public void UpdateTiles(int tileLayer, Vector2I[] positions)
  {
    var positionsConverted = positions.Select(GodotTypeMapper.Map).ToArray();
    autoTileDrawer.UpdateTiles(tileLayer, positionsConverted);
  }

  public new void QueueFree()
  {
    foreach (var tileMapWrapper in tileMapWrappers)
      tileMapWrapper.TileMapLayer.QueueFree();
    base.QueueFree();
  }
}
