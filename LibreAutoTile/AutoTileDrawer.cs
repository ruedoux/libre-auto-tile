using Qwaitumin.LibreAutoTile.Configuration;

namespace Qwaitumin.LibreAutoTile;

/// <summary>
/// Use this interface to implement bindings for drawing tiles. Must be thread safe!
/// </summary>
public interface ITileMapDrawer
{
  public void Clear();
  public void DrawTiles(int tileLayer, IEnumerable<(Vector2 Position, TileData TileData)> positionsToTileData);
}

/// <summary>
/// Class that manages drawing tiles on high level. Thread safe.
/// </summary>
public class AutoTileDrawer(ITileMapDrawer tileMapDrawer, AutoTiler autoTiler)
{
  private readonly HashSet<Task> tasks = [];
  private readonly object taskLock = new();

  public void Clear()
    => autoTiler.Clear();

  public void Wait()
  {
    Task[] tasksCopy;
    lock (taskLock)
    {
      tasksCopy = [.. tasks];
    }
    Task.WhenAll(tasksCopy).Wait();
    ClearFinishedTasks();
  }

  public TileData GetTile(int layer, Vector2 position)
    => autoTiler.GetTile(layer, position);

  public void DrawTilesAsync(int layer, IEnumerable<(Vector2 Position, int TileId)> positionToTileIds)
  {
    ClearFinishedTasks();
    lock (taskLock)
    {
      tasks.Add(Task.Run(() => DrawTiles(layer, positionToTileIds)));
    }
  }

  public void DrawTiles(int layer, IEnumerable<(Vector2 Position, int TileId)> positionToTileIds)
  {
    List<Vector2> positions = [];
    foreach (var (position, tileId) in positionToTileIds)
    {
      autoTiler.PlaceTile(layer, position, tileId);
      positions.Add(position);
    }

    UpdateTiles(layer, [.. positions]);
  }

  public void UpdateTiles(int tileLayer, IEnumerable<Vector2> positions)
  {
    List<(Vector2, TileData)> tileLayerToData = [];
    foreach (var position in positions)
      tileLayerToData.Add(new(position, autoTiler.GetTile(tileLayer, position)));

    tileMapDrawer.DrawTiles(tileLayer, [.. tileLayerToData]);
  }

  private void ClearFinishedTasks()
  {
    lock (taskLock)
    {
      foreach (var task in tasks.Where(task => task.IsCompleted).ToList())
        tasks.Remove(task);
    }
  }
}