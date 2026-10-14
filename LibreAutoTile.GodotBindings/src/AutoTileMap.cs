using Godot;
using LibreAutoTile.Configuration;
using LibreAutoTile.Configuration.Models;
using LibreAutoTile.Tiling;

namespace LibreAutoTile.GodotBindings;

public class AutoTileMap : Node2D
{
  private readonly TileMapWrapper[] tileMapWrappers;
  private readonly AutoTileDrawer autoTileDrawer;
  private readonly TileMapDrawer tileMapDrawer;

  /// <summary>
  /// Constructs the tile map. Must be called on the main thread: this creates Godot
  /// Resources and Nodes (image loading, TileSet/atlas sources, CreateTile, AddChild)
  /// that are not safe to build from a background thread.
  /// Throws <see cref="InvalidOperationException"/> when not called on the main thread.
  /// </summary>
  public AutoTileMap(
    int layerCount,
    AutoTileConfiguration autoTileConfiguration,
    TileShape? tileShape = null)
  {
    if (OS.GetThreadCallerId() != OS.GetMainThreadId())
      throw new InvalidOperationException("AutoTileMap must be constructed on the main thread.");

    autoTileConfiguration.VerifyFiles();

    TileShape resolvedTileShape = tileShape ?? autoTileConfiguration.TileShape;

    tileMapWrappers = new TileMapWrapper[layerCount];
    for (int layer = 0; layer < layerCount; layer++)
    {
      TileMapWrapper tileMapWrapper = new(autoTileConfiguration, resolvedTileShape);
      tileMapWrappers[layer] = tileMapWrapper;
      AddChild(tileMapWrapper.TileMapLayer);
    }

    tileMapDrawer = new TileMapDrawer(tileMapWrappers);
    autoTileDrawer = new(
      tileMapDrawer, new AutoTiler(layerCount, AutoTileConfigurationExtractor.BuildTileIdToTileMaskSearcher(autoTileConfiguration)));
  }

  public Vector2I WorldToMap(Godot.Vector2 localPosition)
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
    => await autoTileDrawer.DrawTilesAsync(layer, ConvertPositions(positionToTileIds));

  public void DrawTiles(int layer, IEnumerable<(Vector2I Position, int TileId)> positionToTileIds)
    => autoTileDrawer.DrawTiles(layer, ConvertPositions(positionToTileIds));

  public void UpdateTiles(int tileLayer, Vector2I[] positions)
  {
    var positionsConverted = positions.Select(GodotTypeMapper.Map).ToArray();
    autoTileDrawer.UpdateTiles(tileLayer, positionsConverted);
  }

  private static (Configuration.Models.Vector2 Position, int TileId)[] ConvertPositions(
    IEnumerable<(Vector2I Position, int TileId)> positionToTileIds)
    => positionToTileIds
      .Select(positionToTileId => (GodotTypeMapper.Map(positionToTileId.Position), positionToTileId.TileId))
      .ToArray();

  public new void QueueFree()
  {
    if (OS.GetThreadCallerId() != OS.GetMainThreadId())
    {
      Callable.From(() => QueueFree()).CallDeferred();
      return;
    }

    foreach (var tileMapWrapper in tileMapWrappers)
      tileMapWrapper.TileMapLayer.QueueFree();
    base.QueueFree();
  }
}
