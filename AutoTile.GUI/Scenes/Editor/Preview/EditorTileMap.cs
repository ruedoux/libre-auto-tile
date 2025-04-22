using Qwaitumin.AutoTile.Configuration;
using Qwaitumin.AutoTile.GodotBindings;

namespace Qwaitumin.AutoTile.GUI.Scenes.Editor.Preview;


public class EditorTileMap
{
  public AutoTileMap? AutoTileMap;

  public void InitializeTileMap(
    string imageDirectoryPath, AutoTileConfiguration autoTileConfiguration)
  {
    AutoTileMap?.QueueFree();
    AutoTileMap = new(1, imageDirectoryPath, autoTileConfiguration);
  }
}