using System.Collections.Generic;
using System.Linq;
using Godot;
using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.GodotBindings;
using Qwaitumin.LibreAutoTile.GUI.GodotBindings;
using Qwaitumin.LibreAutoTile.GUI.Signals;
using Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.Tiles;
using System;
using System.Collections.Immutable;
using Qwaitumin.LibreAutoTile.Tiling;

namespace Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.Preview;


public partial class EditorPreview : MarginContainer, IState
{
  private static readonly PackedScene tileMapTileScene = ResourceLoader.Load<PackedScene>(
    "uid://bagpwruhxb7ka");

  public readonly EventNotifier<bool> EnteredPreview = new();
  public readonly EventNotifier<bool> ExitedPreview = new();
  public TileMapTile? ActiveTile { private set; get; } = null;
  public AutoTileMap? AutoTileMap { private set; get; } = null;

  private Control tileList = null!;

  public override void _Ready()
  {
    tileList = GetNode<Control>("V/ScrollContainer/List");
  }

  // TODO this needs refactor
  public void AddCreatedTiles(
    HashSet<GuiTile> CreatedTiles, AutoTileConfiguration autoTileConfiguration)
  {
    ActiveTile = null;
    foreach (var tileMapTile in tileList.GetChildren())
    {
      tileList.RemoveChild(tileMapTile);
      tileMapTile.QueueFree();
    }

    var defaultMask = TileMask.FromArray([-1, -1, -1, -1, -1, -1, -1, -1]);
    Dictionary<uint, Tuple<Configuration.Models.Vector3, string>> tileIdToAtlasPosition = [];
    foreach (var (tileId, tileDefinition) in autoTileConfiguration.TileDefinitions)
      foreach (var (imageFileName, tileMaskDefinition) in tileDefinition.ImageFileNameToTileMaskDefinition)
        foreach (var (atlasPosition, tileMask) in tileMaskDefinition.AtlasPositionToTileMasks)
          if (TileMask.FromArray(tileMask.SelectMany(e => e).ToArray()) == defaultMask)
            tileIdToAtlasPosition[tileId] = new(atlasPosition, imageFileName);

    foreach (var guiTile in CreatedTiles)
    {
      GodotLogger.Logger.Log(guiTile.TileName);
      var tileMapTile = tileMapTileScene.Instantiate<TileMapTile>();
      tileList.AddChild(tileMapTile);
      tileMapTile.TileSelected.AddObserver(ChangeActiveTile);
      tileMapTile.NameLabel.Text = guiTile.TileName;
      tileMapTile.TileId = guiTile.TileId;

      if (AutoTileMap is null)
      {
        GodotLogger.Logger.LogError("AutoTileMap is null");
        return;
      }

      var tileSize = autoTileConfiguration.TileSize;
      var image = Image.CreateEmpty(tileSize, tileSize, false, Image.Format.Rgba8);
      var texture = (Texture2D)ImageTexture.CreateFromImage(image);
      if (tileIdToAtlasPosition.TryGetValue((uint)guiTile.TileId, out var imageAtlasToImageName))
      {
        var atlasPosition = imageAtlasToImageName.Item1;
        var imageFileName = imageAtlasToImageName.Item2;
        var sourceId = AutoTileMap.GetSourceId(imageFileName);
        var source = AutoTileMap.GetTileMapLayer(0).TileSet.GetSource(sourceId);
        texture = GodotApi.GetTileTexture(
          (TileSetAtlasSource)source,
          new(atlasPosition.X, atlasPosition.Y),
          new(tileSize, tileSize));
      }
      tileMapTile.TextureRectangle.Texture = texture;
    }

    if (CreatedTiles.Count > 0)
      ChangeActiveTile((TileMapTile)tileList.GetChildren().First());
  }

  public void InitializeTileMap(AutoTileConfiguration autoTileConfiguration)
  {
    AutoTileMap?.QueueFree();
    AutoTileMap = new(1, autoTileConfiguration);
  }

  public void InitializeState()
  {
    Show();
    EnteredPreview.NotifyObservers(true);
  }

  public void EndState()
  {
    Hide();
    ExitedPreview.NotifyObservers(true);
  }

  private void ChangeActiveTile(TileMapTile tileMapTile)
  {
    if (ActiveTile is not null)
      ActiveTile.SelectButton.Modulate = Colors.White;
    tileMapTile.SelectButton.Modulate = new(r: 0, g: 2, b: 0);
    ActiveTile = tileMapTile;
    GodotLogger.Logger.Log($"Changed active tile: {tileMapTile.NameLabel.Text}");
  }
}