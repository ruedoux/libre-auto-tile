using System.Diagnostics;
using Godot;
using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.GodotBindings;

namespace Qwaitumin.LibreAutoTile.GodotExample.Scenes;

public partial class Compare : Node2D
{
  private const int DEFAULT_MAP_SIDE = 64;
  private const string CONFIG_PATH = "../resources/configurations/ExampleConfigurationTransient.json";
  private const int LAYER = 0;
  private const int TERRAIN_SET = 0;

  private enum TILES { GRASS = 0, WATER = 1 }

  private CanvasLayer menuLayer = null!;
  private LineEdit xLineEdit = null!;
  private LineEdit yLineEdit = null!;
  private Button builtInButton = null!;
  private Button latButton = null!;
  private Label timeLabel = null!;
  private CameraControl cameraControl = null!;

  public override void _Ready()
  {
    menuLayer = GetNode<CanvasLayer>("MenuLayer");
    xLineEdit = GetNode<LineEdit>("MenuLayer/MarginContainer/VBoxContainer/Settings/VBoxContainer/HBoxContainer/x");
    yLineEdit = GetNode<LineEdit>("MenuLayer/MarginContainer/VBoxContainer/Settings/VBoxContainer/HBoxContainer/y");
    builtInButton = GetNode<Button>("MenuLayer/MarginContainer/VBoxContainer/HBoxContainer/BuiltInButton");
    latButton = GetNode<Button>("MenuLayer/MarginContainer/VBoxContainer/HBoxContainer/LATButton");
    timeLabel = GetNode<Label>("TimeLayer/TimeContainer/TimeLabel");

    builtInButton.Pressed += () => Generate(useBuiltIn: true);
    latButton.Pressed += () => Generate(useBuiltIn: false);
  }

  private void Generate(bool useBuiltIn)
  {
    Vector2I mapSize = new(
      ParseSide(xLineEdit.Text),
      ParseSide(yLineEdit.Text));

    menuLayer.Visible = false;

    if (cameraControl == null)
    {
      cameraControl = new CameraControl();
      AddChild(cameraControl);
    }
    cameraControl.View = new(Vector2I.Zero, mapSize * 16);

    long ms = useBuiltIn ? RenderBuiltIn(mapSize) : RenderLibreAutoTile(mapSize);

    timeLabel.Text = $"{ms} ms";
  }

  private Dictionary<Vector2I, int> GetPositionToTileId(Vector2I mapSize)
  {
    FastNoiseLite noise = new()
    {
      NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex,
      FractalType = FastNoiseLite.FractalTypeEnum.Fbm,
      FractalOctaves = 1,
      Frequency = 0.05f,
      Seed = new Random().Next()
    };

    Dictionary<Vector2I, int> positionToTileId = [];
    for (int x = 0; x < mapSize.X; x++)
      for (int y = 0; y < mapSize.Y; y++)
        if (x > 0 && y > 0 && x < mapSize.X - 1 && y < mapSize.Y - 1)
          positionToTileId[new(x, y)] = noise.GetNoise2D(x, y) < 0.5 ? (int)TILES.GRASS : (int)TILES.WATER;
        else
          positionToTileId[new(x, y)] = (int)TILES.WATER;
    return positionToTileId;
  }

  private long RenderBuiltIn(Vector2I mapSize)
  {
    TileSet tileSet = ResourceLoader.Load<TileSet>("res://Scenes/Comparasion/TileSet.tres");
    TileMapLayer tileMapLayer = new()
    {
      TileSet = tileSet,
      TextureFilter = TextureFilterEnum.Nearest
    };
    AddChild(tileMapLayer);

    var positionToTileId = GetPositionToTileId(mapSize);

    // Just gonna split id 0 and 1 to separate positions
    var positionsGrass = new Godot.Collections.Array<Vector2I>(
      positionToTileId.Where(kv => kv.Value == (int)TILES.GRASS).Select(kv => kv.Key).ToList());
    var positionsWater = new Godot.Collections.Array<Vector2I>(
      positionToTileId.Where(kv => kv.Value == (int)TILES.WATER).Select(kv => kv.Key).ToList());

    Stopwatch stopwatch = Stopwatch.StartNew();
    tileMapLayer.SetCellsTerrainConnect(positionsGrass, TERRAIN_SET, (int)TILES.GRASS);
    tileMapLayer.SetCellsTerrainConnect(positionsWater, TERRAIN_SET, (int)TILES.WATER);
    return stopwatch.ElapsedMilliseconds;
  }

  private long RenderLibreAutoTile(Vector2I mapSize)
  {
    var autoTileConfiguration = AutoTileConfiguration.LoadFromFile(CONFIG_PATH);
    AutoTileMap autoTileMap = new(1, autoTileConfiguration);
    AddChild(autoTileMap);

    var positionToTileId = GetPositionToTileId(mapSize);

    Stopwatch stopwatch = Stopwatch.StartNew();
    autoTileMap.DrawTiles(
      LAYER, [.. positionToTileId.Select(kv => (Position: kv.Key, TileId: kv.Value))]);
    return stopwatch.ElapsedMilliseconds;
  }

  private static int ParseSide(string text)
    => int.TryParse(text, out int value) && value > 0 ? value : DEFAULT_MAP_SIDE;
}
