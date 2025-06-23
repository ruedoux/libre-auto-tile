using Godot;
using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.Tiling;

namespace Qwaitumin.LibreAutoTile.GodotBindings;

public class AutoTileMap : Node2D
{
  private readonly TileMapWrapper tileMapWrapper;
  private readonly AutoTileDrawer autoTileDrawer;
  private readonly int tileSize;

  public AutoTileMap(uint layerCount, AutoTileConfiguration autoTileConfiguration)
  {
    foreach (var (_, tileDefinition) in autoTileConfiguration.TileDefinitions)
      foreach (var (imageFileName, _) in tileDefinition.ImageFileNameToTileMaskDefinition)
        if (!File.Exists(imageFileName))
          throw new FileNotFoundException($"File defined in configuration does not exist: '{imageFileName}'");

    tileMapWrapper = new(autoTileConfiguration);
    autoTileDrawer = new(
      new TileMapDrawer(tileMapWrapper),
      new AutoTiler(layerCount, autoTileConfiguration));
    tileSize = autoTileConfiguration.TileSize;

    AddChild(tileMapWrapper.TileMapLayer);
  }

  public Vector2I WorldToMap(Godot.Vector2 worldPosition)
  {
    int tileXScaledDown = (int)Math.Floor(worldPosition.X / tileSize);
    int tileYScaledDown = (int)Math.Floor(worldPosition.Y / tileSize);
    return new Vector2I(tileXScaledDown, tileYScaledDown);
  }

  public Tiling.TileData GetTile(int layer, Vector2I position)
    => autoTileDrawer.GetTile(layer, GodotTypeMapper.Map(position));

  public void Clear()
    => autoTileDrawer.Clear();

  public void Wait()
    => autoTileDrawer.Wait();

  public void DrawTilesAsync(int layer, IEnumerable<(Vector2I Position, int TileId)> positionToTileIds)
  {
    var positionToTileIdsConverted = positionToTileIds
      .Select(positionToTileId => (GodotTypeMapper.Map(positionToTileId.Position), positionToTileId.TileId))
      .ToArray();
    autoTileDrawer.DrawTilesAsync(layer, positionToTileIdsConverted);
  }

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

  public TileMapLayer GetTileMapLayer()
    => tileMapWrapper.TileMapLayer;

  public new void QueueFree()
  {
    tileMapWrapper.TileMapLayer.QueueFree();
    base.QueueFree();
  }
}