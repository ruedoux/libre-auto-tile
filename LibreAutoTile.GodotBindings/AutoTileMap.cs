using Godot;
using Qwaitumin.LibreAutoTile.Configuration;

namespace Qwaitumin.LibreAutoTile.GodotBindings;

public class AutoTileMap : Node2D
{
  private readonly TileMapWrapper tileMapWrapper;
  private readonly AutoTileDrawer autoTileDrawer;
  private readonly int tileSize;

  public AutoTileMap(
    uint layerCount,
    AutoTileConfiguration autoTileConfiguration)
  {
    AutoTilerComposer autoTilerComposer = new(autoTileConfiguration);
    tileMapWrapper = new(autoTileConfiguration);
    autoTileDrawer = new(
      new TileMapDrawer(tileMapWrapper),
      autoTilerComposer.GetAutoTiler(layerCount));
    tileSize = autoTileConfiguration.TileSize;

    AddChild(tileMapWrapper.TileMapLayer);
  }

  public Vector2I WorldToMap(Godot.Vector2 worldPosition)
  {
    int tileXScaledDown = (int)Math.Floor(worldPosition.X / tileSize);
    int tileYScaledDown = (int)Math.Floor(worldPosition.Y / tileSize);
    return new Vector2I(tileXScaledDown, tileYScaledDown);
  }

  public void Clear()
    => autoTileDrawer.Clear();

  public void Wait()
    => autoTileDrawer.Wait();

  public void DrawTilesAsync(int layer, KeyValuePair<Vector2I, int>[] positionToTileIds)
  {
    var positionToTileIdsConverted = positionToTileIds
      .Select(kvp => new KeyValuePair<Configuration.Vector2, int>(
          GodotTypeMapper.Map(kvp.Key),
          kvp.Value))
      .ToArray();
    autoTileDrawer.DrawTilesAsync(layer, positionToTileIdsConverted);
  }

  public void DrawTiles(int layer, KeyValuePair<Vector2I, int>[] positionToTileIds)
  {
    var positionToTileIdsConverted = positionToTileIds
      .Select(kvp => new KeyValuePair<Configuration.Vector2, int>(
          GodotTypeMapper.Map(kvp.Key),
          kvp.Value))
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