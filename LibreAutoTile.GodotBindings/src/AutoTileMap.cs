using Godot;
using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.Tiling;

namespace Qwaitumin.LibreAutoTile.GodotBindings;

public class AutoTileMap : Node2D
{
  private readonly TileMapWrapper[] tileMapWrappers;
  private readonly AutoTileDrawer autoTileDrawer;
  private readonly int tileSize;
  private readonly TileMapDrawer tileMapDrawer;

  public AutoTileMap(uint layerCount, AutoTileConfiguration autoTileConfiguration)
  {
    foreach (var (_, tileDefinition) in autoTileConfiguration.TileDefinitions)
      foreach (var (imageFileName, _) in tileDefinition.ImageFileNameToTileMaskDefinition)
        if (!File.Exists(imageFileName))
          throw new FileNotFoundException($"File defined in configuration does not exist: '{imageFileName}'");

    tileMapWrappers = new TileMapWrapper[layerCount];
    for (int layer = 0; layer < layerCount; layer++)
    {
      TileMapWrapper tileMapWrapper = new(autoTileConfiguration);
      tileMapWrappers[layer] = tileMapWrapper;
      AddChild(tileMapWrapper.TileMapLayer);
    }

    tileMapDrawer = new TileMapDrawer(tileMapWrappers);
    autoTileDrawer = new(
      tileMapDrawer, new AutoTiler(layerCount, autoTileConfiguration));
    tileSize = autoTileConfiguration.TileSize;
  }

  public Vector2I WorldToMap(Godot.Vector2 worldPosition)
  {
    int tileXScaledDown = (int)Math.Floor(worldPosition.X / tileSize);
    int tileYScaledDown = (int)Math.Floor(worldPosition.Y / tileSize);
    return new Vector2I(tileXScaledDown, tileYScaledDown);
  }

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