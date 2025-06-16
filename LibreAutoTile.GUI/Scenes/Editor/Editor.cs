using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.GUI.Core;
using Qwaitumin.LibreAutoTile.GUI.Core.GodotBindings;
using Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.Options;
using Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.Preview;
using Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.Tiles;
using Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.TileSet;
using Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.Utils;
using Qwaitumin.LibreAutoTile.GUI.Scenes.Utils;
using Qwaitumin.Logging;

namespace Qwaitumin.LibreAutoTile.GUI.Scenes.Editor;

public enum EditorTools { Tiles, Settings, Preview }

public partial class Editor : Control
{
  public const int IMAGE_SCALING = 4;

  private readonly GodotInputListener inputListener = new();
  private readonly CameraControl cameraControl;
  private readonly EditorTileDrawer tileDrawer;
  private readonly MouseLabel mouseLabel;
  private readonly BitmaskContainer bitmaskContainer;
  private readonly List<Control> UiElements = [];

  private StateMachine<EditorTools> toolsStateMachine = null!;
  private Settings.EditorSettings editorSettings = null!;
  private EditorOptions editorOptions = null!;
  private EditorTiles editorTiles = null!;
  private MessageDisplay messageDisplay = null!;
  private EditorLayer editorLayer = null!;
  private EditorPreview editorPreview = null!;

  public Editor()
  {
    cameraControl = GodotApi.AddChild<CameraControl>(this, new());
    tileDrawer = GodotApi.AddChild<EditorTileDrawer>(this, new());
    mouseLabel = GodotApi.AddChild(this, ResourceLoader.Load<PackedScene>("res://Scenes/Utils/MouseLabel.tscn").Instantiate<MouseLabel>());
    bitmaskContainer = GodotApi.AddChild<BitmaskContainer>(this, new());
  }

  public override void _Ready()
  {
    editorSettings = GetNode<Settings.EditorSettings>("CanvasLayer/Window/Tools/V/EditorSettings");
    editorOptions = GetNode<EditorOptions>("CanvasLayer/Window/Tools/V/EditorOptions");
    editorTiles = GetNode<EditorTiles>("CanvasLayer/Window/Tools/V/EditorTiles");
    messageDisplay = GetNode<MessageDisplay>("CanvasLayer/Window/Workspace/MessageDisplay");
    editorLayer = GetNode<EditorLayer>("CanvasLayer/Window/Workspace/V/H/MarginContainer/EditorLayer");
    editorPreview = GetNode<EditorPreview>("CanvasLayer/Window/Tools/V/EditorPreview");
    UiElements.AddRange([GetNode<Control>("CanvasLayer/Window/Tools"), editorLayer]);

    toolsStateMachine = new(
      EditorTools.Tiles,
      new() {
        { EditorTools.Settings, editorSettings },
        { EditorTools.Tiles, editorTiles },
        { EditorTools.Preview, editorPreview }
    });
    GodotApi.FillOptionButtonWithEnum(editorOptions.ToolsOptionsButton, EditorTools.Tiles);

    editorSettings.GridColorObservable.AddObserver(
      (_) => UpdateGrid());
    editorSettings.ScaledTileSizeObservable.AddObservers([
      (_) => UpdateBitmask(),
      (_) => UpdateGrid()]);

    editorTiles.ChangedActiveTile.AddObserver(
      (_) => UpdateBitmask());
    editorTiles.TileColorChanged.AddObserver((_) => UpdateBitmask());
    editorTiles.TileDeleted.AddObservers([
      (guiTile) => bitmaskContainer.RemoveTileId(guiTile.TileId),
      (_) => UpdateBitmask()]);

    editorOptions.ImageRectangleObservable.AddObservers([
      (imageSize) => cameraControl.View = imageSize,
      (_) => UpdateGrid()]);
    editorOptions.ImageTextureObservable.AddObservers([
      (texture) => bitmaskContainer.TileSetTexture.Texture = texture]);
    editorOptions.ToolHasChanged.AddObserver(toolsStateMachine.SwitchStateTo);
    editorOptions.ConfigurationSaved.AddObserver(SaveConfiguration);
    editorOptions.ImageFileObservable.AddObservers([
      (_) => UpdateBitmask(),
      (_) => {cameraControl.Position = Godot.Vector2.Zero;}]);
    editorOptions.ConfigurationCleared.AddObserver((_) => ClearBitmasks());
    editorOptions.ConfigurationLoaded.AddObserver(LoadConfiguration);

    editorSettings.GridColorObservable.NotifyObservers();
    editorSettings.TileSizeObservable.NotifyObservers();

    editorLayer.LayerObservable.AddObserver((_) => UpdateBitmask());

    editorPreview.EnteredPreview.AddObserver((_) => EnterEditorPreview());
    editorPreview.ExitedPreview.AddObserver((_) => ExitEditorPreview());

    inputListener.AddInputMouseMotionAction((_) => UpdateSelectedTile(GetGlobalMousePosition()));
    inputListener.AddInputMouseButtonAction(BitmaskInput);
    inputListener.AddInputMouseMotionAction(BitmaskInput);
    inputListener.AddInputMouseButtonAction(AutoTileMapInput);
    inputListener.AddInputMouseMotionAction(AutoTileMapInput);
  }

  public override void _Input(InputEvent @event)
  {
    if (!GodotApi.IsMouseOnElements([.. UiElements]))
    {
      inputListener.ListenToInput(@event);
      tileDrawer.ShowSelectedTile();
      mouseLabel.Show();
      bitmaskContainer.TileSetBitmaskDrawer.ShowBitmaskGhost();
    }
    else
    {
      tileDrawer.HideSelectedTile();
      mouseLabel.Hide();
      bitmaskContainer.TileSetBitmaskDrawer.HideBitmaskGhost();
    }
  }

  public override void _PhysicsProcess(double delta)
  {
    if (!GodotApi.IsMouseOnElements([.. UiElements]))
      inputListener.ListenToProcess();
  }

  private void ClearBitmasks()
  {
    GodotLogger.Logger.Log("> Starting clearing editor state");
    editorTiles.ClearAll();
    bitmaskContainer.Clear();
    UpdateBitmask();
    GodotLogger.Logger.Log("> Finished clearing editor state");
  }

  private void AutoTileMapInput(InputEventMouse inputEventMouse)
  {
    if (toolsStateMachine.CurrentState != editorPreview)
      return;

    var mouseRightClicked = inputEventMouse.ButtonMask == MouseButtonMask.Right;
    var mouseLeftClicked = inputEventMouse.ButtonMask == MouseButtonMask.Left;
    var mousePosition = GetGlobalMousePosition();
    if (editorPreview.AutoTileMap is null || editorPreview.ActiveTile is null)
      return;

    if (mouseLeftClicked)
      editorPreview.AutoTileMap.DrawTiles(
        0, [new(editorPreview.AutoTileMap.WorldToMap(mousePosition / IMAGE_SCALING), editorPreview.ActiveTile.TileId)]);
    if (mouseRightClicked)
      editorPreview.AutoTileMap.DrawTiles(
        0, [new(editorPreview.AutoTileMap.WorldToMap(mousePosition / IMAGE_SCALING), -1)]);

    if (mouseLeftClicked || mouseRightClicked)
    {
      List<Vector2I> surroundingPositions = [];
      var scaledMousePosition = editorPreview.AutoTileMap.WorldToMap(mousePosition / IMAGE_SCALING);
      for (int x = -1; x < 2; x++)
        for (int y = -1; y < 2; y++)
          surroundingPositions.Add(scaledMousePosition + new Vector2I(x, y));
      editorPreview.AutoTileMap.UpdateTiles(0, [.. surroundingPositions]);
    }
  }

  private void BitmaskInput(InputEventMouse inputEventMouse)
  {
    if (toolsStateMachine.CurrentState != editorTiles)
      return;

    var mousePosition = GetGlobalMousePosition();
    var mousePositionInt = new Vector2I((int)mousePosition.X, (int)mousePosition.Y);
    if (!editorOptions.ImageRectangleObservable.Value.HasPoint(mousePositionInt))
      return;

    var mouseRightClicked = inputEventMouse.ButtonMask == MouseButtonMask.Right;
    var mouseLeftClicked = inputEventMouse.ButtonMask == MouseButtonMask.Left;
    if (mouseRightClicked)
      bitmaskContainer.RemoveBitmask(
        (int)editorLayer.LayerObservable.Value,
        editorOptions.ImageFileObservable.Value,
        mousePositionInt,
        editorSettings.ScaledTileSizeObservable.Value);
    if (mouseLeftClicked && editorTiles.ActiveTile is not null)
      bitmaskContainer.PlaceBitmask(
        (int)editorLayer.LayerObservable.Value,
        editorTiles.ActiveTile.TileId,
        editorOptions.ImageFileObservable.Value,
        mousePositionInt,
        editorSettings.ScaledTileSizeObservable.Value);
    if (mouseRightClicked || mouseLeftClicked)
      UpdateBitmask();
  }

  private void SaveConfiguration(string filePath)
  {
    var configuration = ExtractAutoTileConfiguration();
    var jsonString = configuration.ToJsonString();
    File.WriteAllText(filePath, jsonString);
    GodotLogger.Logger.Log($"Saved configuration to: {filePath}");
    messageDisplay.DisplayText($"[color=green]Saved configuration to: {filePath}[/color]");
  }

  private void LoadConfiguration(string filePath)
  {
    ConfigurationExtractor.LoadConfiguration(filePath, editorTiles, bitmaskContainer);
    UpdateBitmask();
    messageDisplay.DisplayText($"[color=green]Loaded configuration from: {filePath}[/color]");
    GodotLogger.Logger.Log($"Loaded configuration from: {filePath}");
  }

  private AutoTileConfiguration ExtractAutoTileConfiguration()
  {
    EditorContext editorContext = new(
      editorSettings.TileSizeObservable.Value,
      editorTiles.CreatedTiles,
      bitmaskContainer.TileDatabase);

    return ConfigurationExtractor.GetAsAutoTileConfiguration(editorContext);
  }

  private void UpdateBitmask()
    => bitmaskContainer.RedrawBitmask(
      editorOptions.ImageFileObservable.Value,
      (int)editorLayer.LayerObservable.Value,
      editorTiles.CreatedTiles.ToDictionary(x => x.TileId, x => x.TileName),
      editorTiles.GetTileNamesToColors(),
      editorSettings.ScaledTileSizeObservable.Value);

  private void UpdateGrid()
    => tileDrawer.RedrawGrid(
      editorOptions.ImageRectangleObservable.Value,
      editorSettings.GridColorObservable.Value,
      editorSettings.ScaledTileSizeObservable.Value);

  private void UpdateSelectedTile(Godot.Vector2 mousePosition)
  {
    bitmaskContainer.RedrawBitmaskGhost(
      mousePosition,
      editorSettings.ScaledTileSizeObservable.Value,
      new(r: 255f, g: 255f, b: 255f, a: 0.5f));
    tileDrawer.RedrawTile(
      mousePosition,
      editorSettings.SelectionColorObservable.Value,
      editorSettings.ScaledTileSizeObservable.Value);
    var tilePosition = TileSetMath.ScaleDownTilePosition(mousePosition, editorSettings.ScaledTileSizeObservable.Value);
    mouseLabel.DisplayText(tilePosition.ToString());
    mouseLabel.MoveOnMousePosition();
  }

  private void EnterEditorPreview()
  {
    editorPreview.AddCreatedTiles(editorTiles.CreatedTiles);
    editorLayer.Hide();
    bitmaskContainer.Hide();
    tileDrawer.GridDrawNode.Hide();
    editorOptions.ImageUiContainer.Hide();
    editorOptions.ConfigurationUiContainer.Hide();
    cameraControl.View = new(-int.MaxValue / 2, -int.MaxValue / 2, int.MaxValue, int.MaxValue);
    cameraControl.Position = Godot.Vector2.Zero;

    editorPreview.InitializeTileMap(ExtractAutoTileConfiguration());
    AddChild(editorPreview.AutoTileMap);
    if (editorPreview.AutoTileMap is not null)
      editorPreview.AutoTileMap.Scale = new(IMAGE_SCALING, IMAGE_SCALING);
    GodotLogger.Logger.Log($"Entered preview and loaded AutoTileMap");
  }

  private void ExitEditorPreview()
  {
    editorLayer.Show();
    bitmaskContainer.Show();
    tileDrawer.GridDrawNode.Show();
    editorOptions.ImageUiContainer.Show();
    editorOptions.ConfigurationUiContainer.Show();
    cameraControl.View = editorOptions.ImageRectangleObservable.Value;
    cameraControl.Position = Godot.Vector2.Zero;
    UpdateGrid();
    UpdateBitmask();

    RemoveChild(editorPreview.AutoTileMap);
    GodotLogger.Logger.Log($"Exited preview and unloaded AutoTileMap");
  }
}
