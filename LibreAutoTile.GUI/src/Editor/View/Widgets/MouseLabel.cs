using Godot;
using LibreAutoTile.GUI.Settings;

namespace LibreAutoTile.GUI.Editor.View.Widgets;

public partial class MouseLabel : CanvasLayer
{
  private readonly MarginContainer marginContainer;
  private readonly RichTextLabel richTextLabel;

  public MouseLabel(AppSettings settings)
  {
    marginContainer = this.AppendMargin()
      .WithDefaultTheme(settings.UiSettings)
      .WithMargins(settings.UiSettings.MarginMedium);

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
