using Godot;

namespace Qwaitumin.LibreAutoTile.GUI.Views;

public partial class MouseLabel : CanvasLayer
{
  private readonly MarginContainer marginContainer;
  private readonly RichTextLabel richTextLabel;

  public MouseLabel()
  {
    marginContainer = this.AppendMargin()
      .WithDefaultTheme()
      .WithMargins(Settings.MARGIN_MEDIUM);

    richTextLabel = marginContainer.AppendLabel("(0, 0)")
      .FitContent()
      .DisableAutowrap();
    richTextLabel.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
  }

  public override void _Ready()
  {
    ResizeToFitContent();
  }

  public void DisplayText(string text)
  {
    richTextLabel.Text = text;
    ResizeToFitContent();
  }

  public void MoveOnMousePosition()
  {
    marginContainer.GlobalPosition = richTextLabel.GetGlobalMousePosition();
  }

  private void ResizeToFitContent()
  {
    richTextLabel.ResetSize();
    marginContainer.Size = marginContainer.GetCombinedMinimumSize();
  }
}
