using System.Collections.Generic;
using Godot;

namespace Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.TileSet;

public class GuiTileDatabase
{
  private readonly Dictionary<string, Dictionary<Vector2I, GuiTileData>> data = [];

  public void SetPackedTileData(string fileName, Vector2I position, GuiTileData packedTileData)
  {
    if (!data.TryGetValue(fileName, out var positionToPackedTileData))
    {
      positionToPackedTileData = [];
      data[fileName] = positionToPackedTileData;
    }

    positionToPackedTileData[position] = packedTileData;
  }

  public GuiTileData GetPackedTileData(string fileName, Vector2I position)
  {
    if (!data.TryGetValue(fileName, out var positionToPackedTileData))
    {
      positionToPackedTileData = [];
      data[fileName] = positionToPackedTileData;
    }

    if (!positionToPackedTileData.TryGetValue(position, out var packedTileData))
    {
      packedTileData = new();
      positionToPackedTileData[position] = packedTileData;
    }

    return packedTileData;
  }

  public Dictionary<Vector2I, GuiTileData> GetAllByFileName(string fileName)
  {
    if (!data.TryGetValue(fileName, out var positionToPackedTileData))
      return [];

    return positionToPackedTileData;
  }

  public Dictionary<string, Dictionary<Vector2I, GuiTileData>> GetAll()
    => data;

  public void Clear()
    => data.Clear();
}