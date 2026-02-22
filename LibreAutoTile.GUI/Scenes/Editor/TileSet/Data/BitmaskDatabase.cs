using Godot;

namespace Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.TileSet.Data;

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

    if (!positionToPackedTileData.TryGetValue(position, out _))
    {
      BitmaskData? bitmaskData = new();
      positionToPackedTileData[position] = bitmaskData;
    }
  }

  public BitmaskData? GetBitmaskData(string fileName, Vector2I position)
  {
    if (!data.TryGetValue(fileName, out var positionToPackedTileData))
      return null;
    if (!positionToPackedTileData.TryGetValue(position, out var bitmaskData))
      return null;
    return bitmaskData;
  }

  public Dictionary<Vector2I, BitmaskData> GetAllByFileName(string fileName)
  {
    if (!data.TryGetValue(fileName, out var positionToPackedTileData))
      return [];

    return positionToPackedTileData;
  }

  public Dictionary<string, Dictionary<Vector2I, BitmaskData>> GetAll()
    => data;

  public void Clear()
    => data.Clear();
}