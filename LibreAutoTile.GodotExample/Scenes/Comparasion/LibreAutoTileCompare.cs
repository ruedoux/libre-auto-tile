using System;
using System.Diagnostics;
using System.Linq;
using Godot;
using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.GodotBindings;

namespace Qwaitumin.LibreAutoTile.GodotExample.Scenes.Comparasion;

public partial class LibreAutoTileCompare : Node2D
{
  const string CONFIG_PATH = "resources/configurations/ExampleConfigurationTransient.json";
  const int LAYER = 0;

  private readonly Vector2I mapSize = new(512, 512);

  public override void _Ready()
  {
    CameraControl cameraControl = new()
    {
      View = new(Vector2I.Zero, mapSize * 16)
    };
    AddChild(cameraControl);
    Execute();
  }

  public void Execute()
  {
    var autoTileConfiguration = AutoTileConfiguration.LoadFromFile(CONFIG_PATH);
    AutoTileMap autoTileMap = new(1, autoTileConfiguration);
    AddChild(autoTileMap);

    var positionToTileId = Comparasion.GetPositionToTileId(mapSize);

    Stopwatch stopwatch = Stopwatch.StartNew();
    autoTileMap.DrawTiles(
      LAYER, positionToTileId.Select(kv => (Position: kv.Key, TileId: kv.Value)).ToArray());
    GD.Print($"Rendering the map took: {stopwatch.ElapsedMilliseconds}ms");
  }
}