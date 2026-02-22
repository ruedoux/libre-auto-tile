using Godot;

namespace Qwaitumin.LibreAutoTile.GUI.Scenes.Editor;


public partial class MouseLabel : CanvasLayer
{
  private MarginContainer marginContainer = null!;
  private RichTextLabel richTextLabel = null!;

  public override void _Ready()
  {
    marginContainer = GetNode<MarginContainer>("Margin");
    richTextLabel = GetNode<RichTextLabel>("Margin/RichTextLabel");
  }

  public void DisplayText(string text)
  {
    richTextLabel.Text = text;
  }

  public void MoveOnMousePosition()
  {
    marginContainer.GlobalPosition = richTextLabel.GetGlobalMousePosition();
  }
}