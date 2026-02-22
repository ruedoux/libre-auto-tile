using Godot;
using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.GUI.GodotBindings;
using Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.TileSet;
using Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.UI;
using Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.UI.Options;
using Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.UI.Preview;
using Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.UI.Tiles;
using Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.Utils.UI;

namespace Qwaitumin.LibreAutoTile.GUI.Scenes.Editor;

public enum EditorTools { Tiles, Settings, Preview }

public partial class Editor : Control
{
  public const int IMAGE_SCALING = 4;

  private readonly GodotInputListener inputListener = new();
  private readonly CameraControl cameraControl;

  private readonly MouseLabel mouseLabel;
  private readonly TileSetContainer tileSetContainer;
  private readonly List<Control> uiElements = [];

  private StateMachine<EditorTools> toolsStateMachine = null!;
  private UI.Settings.EditorSettings editorSettings = null!;
  private EditorOptions editorOptions = null!;
  private EditorTiles editorTiles = null!;
  private MessageDisplay messageDisplay = null!;
  private EditorLayer editorLayer = null!;
  private EditorPreview editorPreview = null!;
  private Button probabilityButton = null!;

  public Editor()
  {
    cameraControl = GodotApi.AddChild<CameraControl>(this, new());
    mouseLabel = GodotApi.AddChild(this, ResourceLoader.Load<PackedScene>("res://Scenes/Editor/MouseLabel.tscn").Instantiate<MouseLabel>());
    tileSetContainer = GodotApi.AddChild<TileSetContainer>(this, new());
    mouseLabel.Hide();
  }

  public override void _Ready()
  {
    editorSettings = GetNode<UI.Settings.EditorSettings>("CanvasLayer/Window/Tools/V/EditorSettings");
    editorOptions = GetNode<EditorOptions>("CanvasLayer/Window/Tools/V/EditorOptions");
    editorTiles = GetNode<EditorTiles>("CanvasLayer/Window/Tools/V/EditorTiles");
    messageDisplay = GetNode<MessageDisplay>("CanvasLayer/Window/Workspace/MessageDisplay");
    editorLayer = GetNode<EditorLayer>("CanvasLayer/Window/Workspace/V/H/MarginContainer/EditorLayer");
    editorPreview = GetNode<EditorPreview>("CanvasLayer/Window/Tools/V/EditorPreview");
    probabilityButton = GetNode<Button>("CanvasLayer/Window/Tools/V/EditorTiles/V/Probability");
    uiElements.AddRange([GetNode<Control>("CanvasLayer/Window/Tools"), editorLayer]);

    probabilityButton.Pressed += SwitchProbabilityVisibility;
    SwitchProbabilityVisibility();

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
    editorSettings.PropabilityColorObservable.AddObserver(
      tileSetContainer.TileProbability.UpdateFontColor);

    editorTiles.ChangedActiveTile.AddObserver(
      (_) => UpdateBitmask());
    editorTiles.TileColorChanged.AddObserver((_) => UpdateBitmask());
    editorTiles.TileDeleted.AddObservers([
      (guiTile) => tileSetContainer.RemoveTileId(guiTile.TileId),
      (_) => UpdateBitmask()]);
    editorTiles.TileIdChanged.AddObservers([
      (x) => tileSetContainer.ChangeTileId(x.newId, x.oldId),
      (_) => UpdateBitmask()]);

    editorOptions.ImageRectangleObservable.AddObservers([
      (imageSize) => cameraControl.View = imageSize,
      (_) => UpdateGrid()]);
    editorOptions.ImageTextureObservable.AddObserver(tileSetContainer.SetNewTexture);
    editorOptions.ToolHasChanged.AddObserver(toolsStateMachine.SwitchStateTo);
    editorOptions.ConfigurationSaved.AddObserver(SaveConfiguration);
    editorOptions.ImageFileObservable.AddObservers([
      (_) => UpdateBitmask(),
      (_) => {cameraControl.Position = Vector2.Zero;}]);
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
    inputListener.AddInputAction(ChangeProbability);
  }

  public override void _Input(InputEvent @event)
  {
    var isMouseOnUI = GodotApi.IsMouseOnElements([.. uiElements]);
    tileSetContainer.UpdateSelectedTileVisibility(!isMouseOnUI);
    mouseLabel.Visible = !isMouseOnUI;
    if (!isMouseOnUI)
      inputListener.ListenToInput(@event);
  }

  private void ClearBitmasks()
  {
    GodotLogger.LOGGER.Log("> Starting clearing editor state");
    editorTiles.ClearAll();
    tileSetContainer.Clear();
    UpdateBitmask();
    GodotLogger.LOGGER.Log("> Finished clearing editor state");
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
      tileSetContainer.RemoveBitmask(
        (int)editorLayer.LayerObservable.Value,
        editorOptions.ImageFileObservable.Value,
        mousePositionInt,
        editorSettings.ScaledTileSizeObservable.Value);
    if (mouseLeftClicked && editorTiles.ActiveTile is not null)
      tileSetContainer.AddBitmask(
        (int)editorLayer.LayerObservable.Value,
        editorTiles.ActiveTile.TileId,
        editorOptions.ImageFileObservable.Value,
        mousePositionInt,
        editorSettings.ScaledTileSizeObservable.Value);
    if (mouseRightClicked || mouseLeftClicked)
      UpdateBitmask();
  }

  private void SwitchProbabilityVisibility()
  {
    bool visible = tileSetContainer.TileProbability.Visible =
      !tileSetContainer.TileProbability.Visible;
    probabilityButton.Text = visible ? "Hide Probability" : "Show Probability";
  }

  private void ChangeProbability(InputEvent inputEvent)
  {
    if (toolsStateMachine.CurrentState != editorTiles)
      return;
    if (!tileSetContainer.TileProbability.Visible)
      return;
    if (inputEvent is not InputEventKey)
      return;

    var mousePosition = GetGlobalMousePosition();
    var mousePositionInt = new Vector2I((int)mousePosition.X, (int)mousePosition.Y);
    if (Input.IsKeyPressed(Key.Q))
      tileSetContainer.AddProbability(
        (int)editorLayer.LayerObservable.Value,
        editorOptions.ImageFileObservable.Value,
        mousePositionInt,
        1,
        editorSettings.ScaledTileSizeObservable.Value);
    if (Input.IsKeyPressed(Key.W))
      tileSetContainer.AddProbability(
        (int)editorLayer.LayerObservable.Value,
        editorOptions.ImageFileObservable.Value,
        mousePositionInt,
        -1,
        editorSettings.ScaledTileSizeObservable.Value);
  }

  private void SaveConfiguration(string filePath)
  {
    var configuration = ExtractAutoTileConfiguration();
    var jsonString = configuration.ToJsonString();
    File.WriteAllText(filePath, jsonString);
    GodotLogger.LOGGER.Log($"Saved configuration to: {filePath}");
    messageDisplay.DisplayText($"[color=green]Saved configuration to: {filePath}[/color]");
  }

  private void LoadConfiguration(string filePath)
  {
    ConfigurationExtractor.LoadConfiguration(filePath, editorTiles, tileSetContainer);
    UpdateBitmask();
    messageDisplay.DisplayText($"[color=green]Loaded configuration from: {filePath}[/color]");
    GodotLogger.LOGGER.Log($"Loaded configuration from: {filePath}");
  }

  private AutoTileConfiguration ExtractAutoTileConfiguration()
  {
    return ConfigurationExtractor.GetAsAutoTileConfiguration(
      editorTiles.CreatedTiles,
      tileSetContainer.BitmaskDatabase,
      editorSettings.TileSizeObservable.Value);
  }

  private void UpdateBitmask()
    => tileSetContainer.Redraw(
      editorOptions.ImageFileObservable.Value,
      (int)editorLayer.LayerObservable.Value,
      editorTiles.CreatedTiles.ToDictionary(x => x.TileId, x => x.TileName),
      editorTiles.GetTileNamesToColors(),
      editorSettings.ScaledTileSizeObservable.Value);

  private void UpdateGrid()
    => tileSetContainer.UpdateGrid(
      editorOptions.ImageRectangleObservable.Value,
      editorSettings.GridColorObservable.Value,
      editorSettings.ScaledTileSizeObservable.Value);

  private void UpdateSelectedTile(Vector2 mousePosition)
  {
    if (toolsStateMachine.CurrentState == editorTiles)
    {
      tileSetContainer.RedrawBitmaskGhost(
        mousePosition,
        editorSettings.ScaledTileSizeObservable.Value,
        new(r: 255f, g: 255f, b: 255f, a: 0.5f));
    }

    tileSetContainer.TileDrawer.RedrawTile(
      mousePosition,
      editorSettings.SelectionColorObservable.Value,
      editorSettings.ScaledTileSizeObservable.Value);
    var tilePosition = TileSetMath.ScaleDownTilePosition(
      mousePosition, editorSettings.ScaledTileSizeObservable.Value);
    mouseLabel.DisplayText(tilePosition.ToString());
    mouseLabel.MoveOnMousePosition();
  }

  private void EnterEditorPreview()
  {
    editorLayer.Hide();
    tileSetContainer.Hide();
    tileSetContainer.TileDrawer.GridDrawNode.Hide();
    editorOptions.ImageUiContainer.Hide();
    editorOptions.ConfigurationUiContainer.Hide();
    cameraControl.View = new(-int.MaxValue / 2, -int.MaxValue / 2, int.MaxValue, int.MaxValue);
    cameraControl.Position = Vector2.Zero;
    AutoTileConfiguration autoTileConfiguration = ExtractAutoTileConfiguration();
    editorPreview.InitializeTileMap(autoTileConfiguration);
    editorPreview.AddCreatedTiles(
      [.. editorTiles.CreatedTiles.Select(t => (t.TileId, t.TileName))],
      autoTileConfiguration);
    AddChild(editorPreview.AutoTileMap);
    if (editorPreview.AutoTileMap is not null)
      editorPreview.AutoTileMap.Scale = new(IMAGE_SCALING, IMAGE_SCALING);
    GodotLogger.LOGGER.Log($"Entered preview and loaded AutoTileMap");
  }

  private void ExitEditorPreview()
  {
    editorLayer.Show();
    tileSetContainer.Show();
    tileSetContainer.TileDrawer.GridDrawNode.Show();
    editorOptions.ImageUiContainer.Show();
    editorOptions.ConfigurationUiContainer.Show();
    cameraControl.View = editorOptions.ImageRectangleObservable.Value;
    cameraControl.Position = Vector2.Zero;
    UpdateGrid();
    UpdateBitmask();

    RemoveChild(editorPreview.AutoTileMap);
    GodotLogger.LOGGER.Log($"Exited preview and unloaded AutoTileMap");
  }
}
