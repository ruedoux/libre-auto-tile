using Godot;
using LibreAutoTile.GUI.Editor.Model;
using LibreAutoTile.GUI.Editor.View.Drawing;
using LibreAutoTile.GUI.Editor.View.Widgets;
using LibreAutoTile.GUI.Settings;
using TileShape = LibreAutoTile.Configuration.Models.TileShape;

namespace LibreAutoTile.GUI.Editor.View;

public partial class Workspace : Node2D
{
  public readonly CameraControl CameraControl;
  public readonly GridDrawer GridDrawer;
  public readonly BitmaskDrawer BitmaskDrawer;
  public readonly TileProbability TileProbability;
  public readonly TextureRect ImageNode;

  private readonly Node2D imageContainer;
  private readonly DrawNode previewHighlightDrawNode;
  private readonly DrawNode probabilitySelectionDrawNode;
  private readonly MouseLabel mouseLabel;

  public Workspace(AppSettings settings)
  {
    CameraControl = this.AppendChild(new CameraControl(settings));
    mouseLabel = this.AppendChild(new MouseLabel(settings));

    imageContainer = this.AppendChild(new Node2D());
    ImageNode = imageContainer.AppendChild(new TextureRect());
    ImageNode.TextureFilter = TextureFilterEnum.Nearest;

    BitmaskDrawer = this.AppendChild(new BitmaskDrawer());
    GridDrawer = this.AppendChild(new GridDrawer());
    TileProbability = this.AppendChild(new TileProbability(settings));
    TileProbability.Hide();

    previewHighlightDrawNode = this.AppendChild(new DrawNode());
    previewHighlightDrawNode.ZIndex = 100;
    previewHighlightDrawNode.Hide();

    probabilitySelectionDrawNode = this.AppendChild(new DrawNode());
    probabilitySelectionDrawNode.ZIndex = 100;
    probabilitySelectionDrawNode.Hide();
  }

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

  public void SetCanvasVisible(bool visible)
  {
    if (visible)
    {
      ImageNode.Show();
      ImageNode.QueueRedraw();
      GridDrawer.Show();
      BitmaskDrawer.Show();
    }
    else
    {
      ImageNode.Hide();
      GridDrawer.Hide();
      BitmaskDrawer.Hide();
    }
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
    drawNode.Clear();

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
