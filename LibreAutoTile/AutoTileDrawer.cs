using Qwaitumin.LibreAutoTile.Configuration;

namespace Qwaitumin.LibreAutoTile;

public interface ITileMapDrawer
{
  public void Clear();
  public void DrawTiles(int tileLayer, KeyValuePair<Vector2, TileData>[] positionsToTileData);
}

public class AutoTileDrawer(ITileMapDrawer tileMapDrawer, AutoTiler autoTiler)
{
  private readonly HashSet<Task> tasks = [];

  public void Clear()
    => autoTiler.Clear();

  public void Wait()
  {
    Task.WhenAll(tasks).Wait();
    ClearFinishedTasks();
  }

  public void DrawTilesAsync(int layer, KeyValuePair<Vector2, int>[] positionToTileIds)
  {
    ClearFinishedTasks();
    tasks.Add(Task.Run(() => DrawTiles(layer, positionToTileIds)));
  }

  public void DrawTiles(int layer, KeyValuePair<Vector2, int>[] positionToTileIds)
  {
    List<Vector2> positions = [];
    foreach (var (position, tileId) in positionToTileIds)
    {
      autoTiler.PlaceTile(layer, position, tileId);
      positions.Add(position);
    }

    UpdateTiles(layer, [.. positions]);
  }

  public void UpdateTiles(int tileLayer, Vector2[] positions)
  {
    List<KeyValuePair<Vector2, TileData>> tileLayerToData = [];
    foreach (var position in positions)
      tileLayerToData.Add(new(position, autoTiler.GetTile(tileLayer, position)));

    tileMapDrawer.DrawTiles(tileLayer, [.. tileLayerToData]);
  }

  private void ClearFinishedTasks()
  {
    foreach (var task in tasks.Where(task => task.IsCompleted).ToList())
      tasks.Remove(task);
  }
}