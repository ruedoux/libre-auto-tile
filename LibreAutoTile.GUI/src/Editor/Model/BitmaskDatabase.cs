using Godot;

namespace LibreAutoTile.GUI.Editor.Model;

public class BitmaskDatabase
{
  private readonly Dictionary<string, Dictionary<Vector2I, BitmaskData>> data = [];

  public void SetPackedTileData(string fileName, Vector2I position, BitmaskData bitmaskData)
    => GetOrCreatePositionMap(fileName)[position] = bitmaskData;

  public void CreateBitmaskData(string fileName, Vector2I position)
  {
    var positionMap = GetOrCreatePositionMap(fileName);
    if (!positionMap.ContainsKey(position))
      positionMap[position] = new BitmaskData();
  }

  private Dictionary<Vector2I, BitmaskData> GetOrCreatePositionMap(string fileName)
  {
    if (!data.TryGetValue(fileName, out var positionMap))
    {
      positionMap = [];
      data[fileName] = positionMap;
    }
    return positionMap;
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

  public IReadOnlyDictionary<string, Dictionary<Vector2I, BitmaskData>> GetAll()
    => data;

  public void RemapTileIdEverywhere(int oldId, int newId)
  {
    foreach (var positionMap in data.Values)
      foreach (var bitmaskData in positionMap.Values)
        bitmaskData.RemapTileId(oldId, newId);
  }

  public IEnumerable<Vector2I> GetPositionsWithCentreTileId(int tileId)
  {
    foreach (var positionMap in data.Values)
      foreach (var (position, bitmaskData) in positionMap)
        foreach (var layer in bitmaskData.GetLayers())
          if (bitmaskData.GetCentreTileId(layer) == tileId)
          {
            yield return position;
            break;
          }
  }

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
