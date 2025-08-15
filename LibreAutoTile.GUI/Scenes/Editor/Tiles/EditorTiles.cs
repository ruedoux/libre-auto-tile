using Godot;
using Qwaitumin.LibreAutoTile.GUI.GodotBindings;
using Qwaitumin.LibreAutoTile.GUI.Signals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.Tiles;

public partial class EditorTiles : MarginContainer, IState
{
  private static readonly PackedScene tileScene = ResourceLoader.Load<PackedScene>(
    "uid://1pokk3kl6wjp");

  public readonly GodotInputListener InputListener = new();
  public readonly EventNotifier<GuiTile> ChangedActiveTile = new();
  public readonly EventNotifier<GuiTile> TileDeleted = new();
  public readonly EventNotifier<(int newId, int oldId)> TileIdChanged = new();
  public readonly EventNotifier<Color> TileColorChanged = new();
  public GuiTile? ActiveTile { private set; get; } = null;
  public readonly HashSet<GuiTile> CreatedTiles = [];

  private Control tileList = null!;
  private Button addTileButton = null!;


  public override void _Ready()
  {
    tileList = GetNode<Control>("V/ScrollContainer/List");
    addTileButton = GetNode<Button>("V/Add");

    addTileButton.Pressed += AddTile;

    AddTile();
    ActiveTile = CreatedTiles.First();
    ChangeActiveTile(CreatedTiles.First());
  }

  public void ClearAll()
  {
    GodotLogger.Logger.Log("> Starting clearing tiles");
    foreach (var guiTile in CreatedTiles)
      RemoveTile(guiTile.TileName);
    ActiveTile = null;
    GodotLogger.Logger.Log("> Finished clearing tiles");
  }

  public Dictionary<string, Color> GetTileNamesToColors()
    => CreatedTiles.ToDictionary(
      guiTile => guiTile.TileName,
      guiTile => guiTile.ColorPickerButton.Color);

  public void EndState()
    => Hide();

  public void InitializeState()
    => Show();

  public void AddTile(int tileId, string tileName, Color color)
  {
    if (CreatedTiles.Any(guiTile => guiTile.TileId == tileId))
      GodotLogger.LogErrorAndThrow($"Cannot create tile with already taken id: '{tileId}'");
    if (CreatedTiles.Any(guiTile => guiTile.TileName == tileName))
      GodotLogger.LogErrorAndThrow($"Cannot create tile with already taken name: '{tileName}'");

    GuiTile tileInstance = tileScene.Instantiate<GuiTile>();
    tileList.AddChild(tileInstance);

    tileInstance.TileId = tileId;
    tileInstance.TileName = tileName;
    tileInstance.TileNameEdit.Text = tileName;
    tileInstance.TileIdEdit.Text = tileId.ToString();
    tileInstance.ColorPickerButton.Color = color;

    tileInstance.TryDeleteNotifier.AddObserver(RemoveTile);
    tileInstance.TryChangeTileNameNotifier.AddObserver(TryChangeTileName);
    tileInstance.TryChangeTileIdNotifier.AddObserver(TryChangeTileId);
    tileInstance.SelectActiveTileNotifier.AddObserver(ChangeActiveTile);
    tileInstance.ColorPickerButton.ColorChanged += TileColorChanged.NotifyObservers;

    CreatedTiles.Add(tileInstance);
    GodotLogger.Logger.Log($"Added new tile: {tileName}");
  }

  private void AddTile()
  {
    Random random = new();
    AddTile(
      GetNextFreeTileId(),
      GetNewTileName(new(CreatedTiles.Select(x => x.TileName))),
      new Color(
        r: (float)random.NextDouble(),
        g: (float)random.NextDouble(),
        b: (float)random.NextDouble(),
        a: 0.7f));
  }

  private void ChangeActiveTile(GuiTile tile)
  {
    if (ActiveTile is not null)
      ActiveTile.SelectButton.Modulate = Colors.White;
    tile.SelectButton.Modulate = new(r: 0, g: 2, b: 0);
    ActiveTile = tile;
    ChangedActiveTile.NotifyObservers(tile);
    GodotLogger.Logger.Log($"Changed active tile: {tile.TileName}");
  }

  private void TryChangeTileName(Tuple<GuiTile, string> tileAndName)
  {
    GuiTile tile = tileAndName.Item1;
    string newTileName = tileAndName.Item2;
    string oldTileName = tile.TileName;

    Dictionary<string, GuiTile> tileNamesToGuiTiles = new(
      CreatedTiles.ToDictionary(guiTile => guiTile.TileName, guiTile => guiTile));
    if (tileNamesToGuiTiles.ContainsKey(newTileName))
    {
      if (tileNamesToGuiTiles[newTileName] == tile) return;
      newTileName = GetNewTileName(new(tileNamesToGuiTiles.Keys), newTileName + "-copy");
    }

    tile.TileName = newTileName;
    tile.TileNameEdit.Text = newTileName;
    GodotLogger.Logger.Log($"Changed tile name from {oldTileName} to {newTileName}");
  }

  private void TryChangeTileId(Tuple<GuiTile, string> tileAndName)
  {
    GuiTile tile = tileAndName.Item1;
    int newId = InputSanitizer.SanitizeInt(tileAndName.Item2);
    int oldId = tile.TileId;

    if (newId < 0)
      newId = 0;

    Dictionary<int, GuiTile> tileIdsToGuiTiles = new(
      CreatedTiles.ToDictionary(guiTile => guiTile.TileId, guiTile => guiTile));
    if (tileIdsToGuiTiles.ContainsKey(newId))
    {
      if (tileIdsToGuiTiles[newId] == tile) return;
      newId = GetNextFreeTileId();
    }

    tile.TileId = newId;
    tile.TileIdEdit.Text = newId.ToString();
    TileIdChanged.NotifyObservers(new(newId, oldId));
    GodotLogger.Logger.Log($"Changed tile id from {newId} to {oldId}");
  }

  private void RemoveTile(string tileName)
  {
    var tileToDelete = CreatedTiles.FirstOrDefault(guiTile => guiTile.TileName == tileName);
    if (tileToDelete is null)
      GodotLogger.LogErrorAndThrow($"Tile cannot be deleted, tile name not found: '{tileName}'");

    tileList.RemoveChild(tileToDelete);
    TileDeleted.NotifyObservers(tileToDelete);
    CreatedTiles.Remove(tileToDelete);
    tileToDelete.ColorPickerButton.ColorChanged -= TileColorChanged.NotifyObservers;
    tileToDelete.QueueFree();

    if (tileToDelete == ActiveTile)
      ActiveTile = null;
    GodotLogger.Logger.Log($"Removed tile {tileName}");
  }

  private int GetNextFreeTileId()
  {
    var assignedIdsSet = CreatedTiles
      .Select(guiTile => guiTile.TileId)
      .ToHashSet();

    int nextFreeId = 0;
    while (assignedIdsSet.Contains(nextFreeId))
      nextFreeId++;

    return nextFreeId;
  }

  private static string GetNewTileName(HashSet<string> names, string defaultName = "Tile")
  {
    string newName = defaultName;
    int index = 0;
    while (names.Contains(newName))
      newName = defaultName + index++.ToString();
    return newName;
  }
}
