using Godot;
using Qwaitumin.LibreAutoTile.GUI.Models;
using TileShape = Qwaitumin.LibreAutoTile.Configuration.Models.TileShape;

namespace Qwaitumin.LibreAutoTile.GUI.Views;

public partial class EditorScene : Control
{
  public readonly CameraControl CameraControl;
  public readonly GridDrawer GridDrawer;
  private readonly Node2D imageContainer;
  private readonly DrawNode previewHighlightDrawNode;
  private readonly DrawNode probabilitySelectionDrawNode;
  private readonly MouseLabel mouseLabel;
  public readonly GodotInputListener InputListener = new();

  public readonly Button SelectImageButton;
  public readonly Button SaveButton;
  public readonly Button LoadButton;
  public readonly Button ClearButton;
  public readonly TabContainer OptionToolsTabs;
  public readonly EditorSettings SettingsPanel;
  public readonly EditorTiles TilesPanel;
  public readonly EditorProbability ProbabilityPanel;
  public readonly EditorPreview PreviewPanel;

  public readonly TextureRect ImageNode;
  public readonly FileDialog SelectImageDialog;
  public readonly FileDialog SaveConfigurationDialog;
  public readonly FileDialog LoadConfigurationDialog;

  public readonly BitmaskDrawer BitmaskDrawer;
  public readonly TileProbability TileProbability;
  public readonly EditorLayer LayerControl;
  public readonly MessageDisplay MessageDisplay;

  public EditorScene()
  {
    CameraControl = this.AppendChild(new CameraControl());
    mouseLabel = this.AppendChild(new MouseLabel());

    imageContainer = this.AppendChild(new Node2D());
    ImageNode = imageContainer.AppendChild(new TextureRect());
    ImageNode.TextureFilter = TextureFilterEnum.Nearest;

    BitmaskDrawer = this.AppendChild(new BitmaskDrawer());
    GridDrawer = this.AppendChild(new GridDrawer());
    TileProbability = this.AppendChild(new TileProbability());
    TileProbability.Visible = false;

    previewHighlightDrawNode = this.AppendChild(new DrawNode());
    previewHighlightDrawNode.ZIndex = 100;
    previewHighlightDrawNode.Hide();

    probabilitySelectionDrawNode = this.AppendChild(new DrawNode());
    probabilitySelectionDrawNode.ZIndex = 100;
    probabilitySelectionDrawNode.Hide();

    SelectImageDialog = this.AppendChild(new FileDialog()).WithDefaultTheme();
    SelectImageDialog.Title = "Open a File";
    SelectImageDialog.FileMode = FileDialog.FileModeEnum.OpenFile;
    SelectImageDialog.Access = FileDialog.AccessEnum.Filesystem;
    SelectImageDialog.Filters = ["*.jpg", "*.png"];

    SaveConfigurationDialog = this.AppendChild(new FileDialog()).WithDefaultTheme();
    SaveConfigurationDialog.Title = "Save Configuration";
    SaveConfigurationDialog.FileMode = FileDialog.FileModeEnum.SaveFile;
    SaveConfigurationDialog.Access = FileDialog.AccessEnum.Filesystem;
    SaveConfigurationDialog.Filters = ["*.json"];

    LoadConfigurationDialog = this.AppendChild(new FileDialog()).WithDefaultTheme();
    LoadConfigurationDialog.Title = "Open a File";
    LoadConfigurationDialog.OkButtonText = "Open";
    LoadConfigurationDialog.FileMode = FileDialog.FileModeEnum.OpenFile;
    LoadConfigurationDialog.Access = FileDialog.AccessEnum.Filesystem;
    LoadConfigurationDialog.Filters = ["*.json"];

    var sc = this.AppendChild(new CanvasLayer())
      .AppendChild(new SplitContainer())
      .FullRect().WithDefaultTheme();

    OptionToolsTabs = sc.AppendTabContainer()
      .ExpandHorizontal()
      .ExpandVertical()
      .StretchRatio(0.45f);

    var settingsPanel = new EditorSettings();
    SettingsPanel = settingsPanel;
    OptionToolsTabs.AddTab(settingsPanel, "Settings");

    var tilesPanel = new EditorTiles();
    TilesPanel = tilesPanel;
    OptionToolsTabs.AddTab(tilesPanel, "Tiles");

    ProbabilityPanel = OptionToolsTabs.AddTab(new EditorProbability(), "Chance");
    PreviewPanel = OptionToolsTabs.AddTab(new EditorPreview(), "Preview");

    var workspace = sc.AppendVBox().ExpandHorizontal().ExpandVertical();

    var topBar = workspace.AppendMargin().ExpandHorizontal().WithMargins(0, Settings.MARGIN_BIG, Settings.MARGIN_MEDIUM, Settings.MARGIN_MEDIUM);
    var topBarHbox = topBar.AppendHBox().ExpandHorizontal();

    SelectImageButton = topBarHbox.AppendButton("Select Image").ExpandHorizontal();
    SaveButton = topBarHbox.AppendButton("Save").ExpandHorizontal();
    LoadButton = topBarHbox.AppendButton("Load").ExpandHorizontal();
    ClearButton = topBarHbox.AppendButton("Clear").ExpandHorizontal();

    MessageDisplay = workspace.AppendChild(new MessageDisplay()).ExpandHorizontal();

    var workspaceArea = workspace.AppendVBox().ExpandFill();

    var bottomBar = workspace.AppendMargin().ExpandHorizontal()
      .WithMargins(0, Settings.MARGIN_BIG, Settings.MARGIN_MEDIUM, Settings.MARGIN_MEDIUM);
    var bottomBarHbox = bottomBar.AppendHBox().ExpandHorizontal();
    bottomBarHbox.Alignment = BoxContainer.AlignmentMode.End;
    LayerControl = bottomBarHbox.AppendChild(new EditorLayer());
  }

  public override void _Ready()
  {
    App.Run(this);
  }

  public override void _Input(InputEvent @event)
    => InputListener.ListenToInput(@event);

  public void SetImage(Texture2D texture)
  {
    ImageNode.Texture = texture;
    ImageNode.Size = texture.GetSize();
  }

  public void ClearImage()
  {
    ImageNode.Texture = null;
    ImageNode.Size = Vector2.Zero;
  }

  public void RedrawGrid(Rect2I size, Color color, int scaledTileSize)
    => GridDrawer.RedrawSquareGrid(size, color, scaledTileSize);

  public void RedrawTile(Vector2I snappedTilePosition, Color color, int scaledTileSize, TileShape shape)
    => GridDrawer.RedrawTile(snappedTilePosition, color, scaledTileSize, shape);

  public void DisplayTileLabel(string text)
  {
    mouseLabel.DisplayText(text);
    mouseLabel.MoveOnMousePosition();
  }

  public void SetMouseLabelVisible(bool visible)
    => mouseLabel.Visible = visible;

  public void SetCameraView(Rect2I view)
  {
    CameraControl.View = view;
    CameraControl.Position = Vector2.Zero;
  }

  public void SetInfiniteCameraView()
  {
    CameraControl.View = new(-int.MaxValue / 2, -int.MaxValue / 2, int.MaxValue, int.MaxValue);
    CameraControl.Position = Vector2.Zero;
  }

  public void HideWorkspace()
  {
    ImageNode.Hide();
    GridDrawer.Hide();
    BitmaskDrawer.Hide();
    LayerControl.Hide();
  }

  public void ShowWorkspace()
  {
    ImageNode.Show();
    ImageNode.QueueRedraw();
    GridDrawer.Show();
    BitmaskDrawer.Show();
    LayerControl.Show();
  }

  public void ShowPreviewHighlight()
    => previewHighlightDrawNode.Show();

  public void HidePreviewHighlight()
  {
    previewHighlightDrawNode.Clear();
    previewHighlightDrawNode.Hide();
  }

  public void RedrawPreviewHighlight(
    Vector2 tileTopLeft, Color color, int scaledTileSize, TileShape shape)
    => RedrawHighlight(previewHighlightDrawNode, tileTopLeft, color, scaledTileSize, shape);

  public void RedrawProbabilitySelection(Vector2I snappedTilePosition, Color color, int scaledTileSize)
  {
    RedrawHighlight(probabilitySelectionDrawNode, snappedTilePosition, color, scaledTileSize, TileShape.Square);
    probabilitySelectionDrawNode.Show();
  }

  public void ClearProbabilitySelection()
  {
    probabilitySelectionDrawNode.Clear();
    probabilitySelectionDrawNode.Hide();
  }

  private static void RedrawHighlight(
    DrawNode drawNode, Vector2 tileTopLeft, Color color, int scaledTileSize, TileShape shape)
  {
    var borderWidth = TileSetMath.BorderWidth(scaledTileSize);
    if (shape == TileShape.Isometric)
    {
      var vertices = TileSetMath.GetTileOutlineVertices(tileTopLeft, scaledTileSize, shape);
      drawNode.DrawPolygon(vertices, color, width: borderWidth);
    }
    else
    {
      drawNode.DrawRectangle(new Rect2I((Vector2I)tileTopLeft, new Vector2I(scaledTileSize, scaledTileSize)), color, width: borderWidth);
    }
    drawNode.QueueRedraw();
  }
}
