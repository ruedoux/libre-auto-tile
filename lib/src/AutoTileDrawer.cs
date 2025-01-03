using System.Numerics;

namespace Qwaitumin.AutoTile;

public interface ITileMapDrawer
{
  public void Clear();
  public void DrawTiles(int tileLayer, KeyValuePair<Vector2, TileData?>[] positionsToTileData);
}

public class AutoTileDrawer
{
  private readonly ITileMapDrawer tileMapDrawer;
  private readonly AutoTiler autoTiler;
  private readonly HashSet<Task> tasks = new();

  public AutoTileDrawer(ITileMapDrawer tileMapDrawer, AutoTiler autoTiler)
  {
    this.tileMapDrawer = tileMapDrawer;
    this.autoTiler = autoTiler;
  }

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
    List<Vector2> positions = new();
    foreach (var (position, tileId) in positionToTileIds)
    {
      autoTiler.PlaceTile(layer, position, tileId);
      positions.Add(position);
    }

    UpdateTiles(layer, positions.ToArray());
  }

  public void UpdateTiles(int tileLayer, Vector2[] positions)
  {
    List<KeyValuePair<Vector2, TileData?>> tileLayerToData = new();
    foreach (var position in positions)
      tileLayerToData.Add(new(position, autoTiler.GetTile(tileLayer, position)));

    tileMapDrawer.DrawTiles(tileLayer, tileLayerToData.ToArray());
  }

  private void ClearFinishedTasks()
  {
    List<Task> tasksToRemove = new();
    foreach (var task in tasks)
      if (task.IsCompleted)
        tasksToRemove.Add(task);

    foreach (var task in tasksToRemove)
      tasks.Remove(task);
  }
}