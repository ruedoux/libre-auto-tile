using Godot;
using LibreAutoTile.GUI.Settings;

namespace LibreAutoTile.GUI.Editor.View.Widgets;

public partial class PreviewTile : PanelContainer
{
  public readonly Button SelectButton;
  public readonly RichTextLabel NameLabel;
  public readonly TextureRect TextureRectangle;
  public int TileId;

  public event Action<PreviewTile>? TileSelected;

  public PreviewTile(AppSettings settings)
  {
    var margin = this.AppendMargin().ExpandHorizontal().WithMargins(settings.UiSettings.MarginMedium);
    var hbox = margin.AppendHBox().ExpandHorizontal();

    SelectButton = hbox.AppendButton("o").ExpandVertical();

    NameLabel = hbox.AppendLabel("")
      .ExpandFill()
      .FitContent()
      .DisableAutowrap();

    TextureRectangle = new TextureRect()
    {
      CustomMinimumSize = new Vector2(64, 64),
      StretchMode = TextureRect.StretchModeEnum.Scale,
      TextureFilter = TextureFilterEnum.Nearest
    };
    hbox.AddChild(TextureRectangle);

    SelectButton.Pressed += () => TileSelected?.Invoke(this);
  }
}
