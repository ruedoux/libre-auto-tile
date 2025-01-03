using System.Collections.Generic;
using System.Linq;
using Godot;
using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.GodotBindings;
using Qwaitumin.LibreAutoTile.GUI.Core;
using Qwaitumin.LibreAutoTile.GUI.Core.GodotBindings;
using Qwaitumin.LibreAutoTile.GUI.Core.Signals;
using Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.Tiles;

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

  public void AddCreatedTiles(HashSet<GuiTile> CreatedTiles)
  {
    ActiveTile = null;
    foreach (var tileMapTile in tileList.GetChildren())
    {
      tileList.RemoveChild(tileMapTile);
      tileMapTile.QueueFree();
    }

    foreach (var guiTile in CreatedTiles)
    {
      GodotLogger.Logger.Log(guiTile.TileName);
      var tileMapTile = tileMapTileScene.Instantiate<TileMapTile>();
      tileList.AddChild(tileMapTile);
      tileMapTile.TileSelected.AddObserver(ChangeActiveTile);
      tileMapTile.NameLabel.Text = guiTile.TileName;
      tileMapTile.TileId = guiTile.TileId;
      tileMapTile.ColorRectangle.Color = guiTile.ColorPickerButton.Color;
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