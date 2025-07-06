using System.Diagnostics;
using System.Linq;
using Godot;

namespace Qwaitumin.LibreAutoTile.GodotExample.Scenes;

public partial class BuiltInCompare : Node2D
{
  Vector2I mapSize = new(512, 512);
  const int TERRAIN_SET = 0;

  public override void _Ready()
  {
    CameraControl cameraControl = new()
    {
      View = new(Vector2I.Zero, Comparasion.MAP_SIZE * 16)
    };
    AddChild(cameraControl);
    Execute();
  }

  public void Execute()
  {
    TileSet tileSet = ResourceLoader.Load<TileSet>("res://Scenes/Comparasion/TileSet.tres");
    TileMapLayer tileMapLayer = new()
    {
      TileSet = tileSet,
      TextureFilter = TextureFilterEnum.Nearest
    };
    AddChild(tileMapLayer);

    var positionToTileId = Comparasion.GetPositionToTileId();

    // Just gonna split id 0 and 1 to separate positions
    var positionsGrass = new Godot.Collections.Array<Vector2I>(
      positionToTileId.Where(kv => kv.Value == (int)Comparasion.TILES.GRASS).Select(kv => kv.Key).ToList());
    var positionsWater = new Godot.Collections.Array<Vector2I>(
      positionToTileId.Where(kv => kv.Value == 1).Select(kv => kv.Key).ToList());

    Stopwatch stopwatch = Stopwatch.StartNew();
    tileMapLayer.SetCellsTerrainConnect(positionsGrass, TERRAIN_SET, (int)Comparasion.TILES.GRASS);
    tileMapLayer.SetCellsTerrainConnect(positionsWater, TERRAIN_SET, (int)Comparasion.TILES.WATER);
    GD.Print($"Rendering the map took: {stopwatch.ElapsedMilliseconds}ms");
  }
}