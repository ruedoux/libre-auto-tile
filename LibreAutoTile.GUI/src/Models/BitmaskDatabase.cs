using Godot;

namespace Qwaitumin.LibreAutoTile.GUI.Models;

public class BitmaskDatabase
{
  private readonly Dictionary<string, Dictionary<Vector2I, BitmaskData>> data = [];

  public void SetPackedTileData(string fileName, Vector2I position, BitmaskData bitmaskData)
  {
    if (!data.TryGetValue(fileName, out var positionToPackedTileData))
    {
      positionToPackedTileData = [];
      data[fileName] = positionToPackedTileData;
    }

    positionToPackedTileData[position] = bitmaskData;
  }

  public void CreateBitmaskData(string fileName, Vector2I position)
  {
    if (!data.TryGetValue(fileName, out var positionToPackedTileData))
    {
      positionToPackedTileData = [];
      data[fileName] = positionToPackedTileData;
    }

    if (!positionToPackedTileData.ContainsKey(position))
      positionToPackedTileData[position] = new BitmaskData();
  }

  public BitmaskData? GetBitmaskData(string fileName, Vector2I position)
  {
    if (!data.TryGetValue(fileName, out var positionToPackedTileData))
      return null;
    if (!positionToPackedTileData.TryGetValue(position, out var bitmaskData))
      return null;
    return bitmaskData;
  }

  public IReadOnlyDictionary<Vector2I, BitmaskData> GetAllByFileName(string fileName)
  {
    if (!data.TryGetValue(fileName, out var positionToPackedTileData))
      return new Dictionary<Vector2I, BitmaskData>();

    return positionToPackedTileData;
  }

  public IReadOnlyDictionary<string, IReadOnlyDictionary<Vector2I, BitmaskData>> GetAll()
    => data.ToDictionary(
      kvp => kvp.Key,
      kvp => (IReadOnlyDictionary<Vector2I, BitmaskData>)kvp.Value);

  public bool RemoveBitmaskData(string fileName, Vector2I position)
  {
    if (!data.TryGetValue(fileName, out var positionToPackedTileData))
      return false;
    if (!positionToPackedTileData.Remove(position))
      return false;

    if (positionToPackedTileData.Count == 0)
      data.Remove(fileName);

    return true;
  }

  public void Clear()
    => data.Clear();
}
