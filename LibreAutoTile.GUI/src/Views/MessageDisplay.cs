using Godot;

namespace Qwaitumin.LibreAutoTile.GUI.Views;

public partial class MessageDisplay : MarginContainer
{
  private readonly RichTextLabel richTextLabel;
  private Tween? fadeTween;

  public MessageDisplay()
  {
    richTextLabel = this.AppendLabel("");
    richTextLabel.BbcodeEnabled = true;
    richTextLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
    richTextLabel.SizeFlagsHorizontal = SizeFlags.ExpandFill;
    richTextLabel.CustomMinimumSize = Vector2.Zero;
    richTextLabel.Modulate = new Color(1, 1, 1, 0);

    ClipContents = true;
  }

  public override void _Ready()
  {
    float lineHeight = richTextLabel.GetThemeDefaultFont()
      .GetHeight(richTextLabel.GetThemeDefaultFontSize());
    CustomMinimumSize = new Vector2(0, lineHeight);
  }

  public void DisplayText(string text, int holdMs = 3000, int fadeMs = 2000)
  {
    fadeTween?.Kill();
    richTextLabel.Text = text;
    richTextLabel.Modulate = new Color(1, 1, 1, 1);
    fadeTween = CreateTween();
    fadeTween.TweenInterval(holdMs / 1000.0);
    fadeTween.TweenProperty(richTextLabel, "modulate:a", 0.0, fadeMs / 1000.0);
  }
}
