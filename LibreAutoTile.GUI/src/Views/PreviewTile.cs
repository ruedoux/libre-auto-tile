using Godot;

namespace Qwaitumin.LibreAutoTile.GUI.Views;

public partial class PreviewTile : PanelContainer
{
  public readonly Button SelectButton;
  public readonly RichTextLabel NameLabel;
  public readonly TextureRect TextureRectangle;
  public int TileId;

  public event Action<PreviewTile>? TileSelected;

  public PreviewTile()
  {
    var margin = this.AppendMargin().ExpandHorizontal().WithMargins(Settings.MARGIN_MEDIUM);
    var hbox = margin.AppendHBox().ExpandHorizontal();

    SelectButton = hbox.AppendButton("o").ExpandVertical();

    NameLabel = hbox.AppendLabel("")
      .ExpandHorizontal()
      .ExpandVertical()
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
