using Godot;

namespace LibreAutoTile.GUI.Settings;

public class UiSettings
{
  public static readonly Vector2I[] STANDARD_RESOLUTIONS =
  [
    new(3840, 2160),
    new(2560, 1440),
    new(1920, 1080),
    new(1600, 900),
    new(1280, 720),
    new(640, 360),
  ];
  private static readonly Font FONT = ResourceLoader.Load<FontFile>("res://resources/SpaceMono-Regular.ttf");

  public int FontSize = 32;
  public float DialogScreenRatio = 0.7f;
  public int DialogThumbnailSize = 128;
  public int BorderWidth = 2;
  public int CornerRadius = 8;
  public int MarginSmall = 4;
  public int MarginMedium = 8;
  public int MarginBig = 16;
  public Color GuiColor = new(0.1f, 0.1f, 0.1f);
  public Color BackgroundColor = Colors.Black;

  public readonly Theme Theme;

  private sealed record StyleDefinition(
    string ControlType, string State, StyleBoxFlat Style,
    Func<Color, Color> Background, Func<Color, Color>? Border = null);

  private StyleDefinition[] styles = [];
  private (string ControlType, string Name, int Value)[] constants = [];

  public UiSettings()
  {
    Theme = CreateTheme();
    Refresh();
  }

  public void Refresh()
  {
    RefreshStyles(GuiColor);
    RenderingServer.SetDefaultClearColor(BackgroundColor);
    Theme.DefaultFontSize = FontSize;
  }

  private Theme CreateTheme()
  {
    styles = BuildStyles();
    constants = BuildConstants();
    RefreshStyles(GuiColor);

    var theme = new Theme
    {
      DefaultFont = FONT,
      DefaultFontSize = FontSize
    };
    foreach (var style in styles)
      theme.SetStylebox(style.State, style.ControlType, style.Style);
    foreach (var (controlType, name, value) in constants)
      theme.SetConstant(name, controlType, value);

    return theme;
  }

  private void RefreshStyles(Color background)
  {
    foreach (var style in styles)
    {
      style.Style.BgColor = style.Background(background);
      if (style.Border is not null)
        style.Style.BorderColor = style.Border(background);
    }
  }

  private StyleDefinition[] BuildStyles()
    =>
    [
      new("Button", "normal", CreateButtonStyle(), bg => bg, bg => bg.Lightened(0.25f)),
      new("Button", "hover", CreateButtonStyle(), bg => bg.Lightened(0.1f), bg => bg.Lightened(0.25f)),
      new("Button", "pressed", CreateButtonStyle(), bg => bg.Darkened(0.25f), bg => bg.Lightened(0.25f)),
      new("Button", "focus", CreateButtonStyle(), bg => bg, _ => new(1f, 1f, 1f)),
      new("Button", "disabled", CreateButtonStyle(), bg => bg.Darkened(0.25f), bg => bg.Lightened(0.25f)),
      new("CheckButton", "normal", CreateCheckButtonStyle(), _ => Colors.Transparent),
      new("CheckButton", "hover", CreateCheckButtonStyle(), _ => Colors.Transparent),
      new("CheckButton", "pressed", CreateCheckButtonStyle(), _ => Colors.Transparent),
      new("CheckButton", "focus", CreateCheckButtonStyle(), _ => Colors.Transparent),
      new("CheckButton", "disabled", CreateCheckButtonStyle(), _ => Colors.Transparent),
      new("ColorPickerButton", "normal", CreateColorPickerStyle(), bg => bg),
      new("ColorPickerButton", "hover", CreateColorPickerStyle(), bg => bg.Lightened(0.1f)),
      new("ColorPickerButton", "pressed", CreateColorPickerStyle(), bg => bg.Darkened(0.25f)),
      new("ColorPickerButton", "focus", CreateColorPickerStyle(), bg => bg),
      new("ColorPickerButton", "disabled", CreateColorPickerStyle(), bg => bg.Darkened(0.25f)),
      new("TabContainer", "tab_selected", CreateTabStyle(), bg => bg),
      new("TabContainer", "tab_unselected", CreateTabStyle(), bg => bg.Darkened(0.25f)),
      new("TabContainer", "tab_hovered", CreateTabStyle(), bg => bg.Lightened(0.1f)),
      new("TabContainer", "tab_disabled", CreateTabStyle(), bg => bg.Darkened(0.25f)),
      new("TabContainer", "panel", new StyleBoxFlat().WithContentMargins(MarginMedium), bg => bg),
      new("TabContainer", "tabbar_background", new(), bg => bg.Darkened(0.4f)),
      new("RichTextLabel", "normal", CreateDefaultStyle(), _ => Colors.Transparent, _ => Colors.Transparent),
      new("AcceptDialog", "panel", CreateDialogStyle(), bg => bg, bg => bg.Lightened(0.25f)),
      new("PopupPanel", "panel", CreateDialogStyle(), bg => bg, bg => bg.Lightened(0.25f)),
      new("LineEdit", "normal", CreateLineEditStyle(), bg => bg.Darkened(0.25f), bg => bg.Lightened(0.25f)),
      new("LineEdit", "focus", CreateLineEditStyle(), bg => bg.Darkened(0.1f), _ => new(1f, 1f, 1f)),
      new("LineEdit", "read_only", CreateLineEditStyle(), bg => bg.Darkened(0.25f), bg => bg.Lightened(0.25f)),
      new("ItemList", "panel", CreateItemListStyle(), bg => bg.Darkened(0.25f), bg => bg.Lightened(0.25f)),
      new("ItemList", "focus", CreateItemListStyle(), bg => bg.Darkened(0.1f), _ => new(1f, 1f, 1f)),
      new("ItemList", "selected", CreateItemSelectionStyle(), bg => bg.Lightened(0.2f)),
      new("ItemList", "selected_focus", CreateItemSelectionStyle(), bg => bg.Lightened(0.2f)),
      new("ItemList", "hovered", CreateItemSelectionStyle(), bg => bg.Lightened(0.1f)),
      new("ItemList", "hovered_selected", CreateItemSelectionStyle(), bg => bg.Lightened(0.2f)),
      new("PopupMenu", "panel", CreateDialogStyle(), bg => bg, bg => bg.Lightened(0.25f)),
      new("PopupMenu", "hover", CreateItemSelectionStyle(), bg => bg.Lightened(0.2f)),
    ];

  private (string, string, int)[] BuildConstants()
    => [("FileDialog", "thumbnail_size", DialogThumbnailSize)];

  private StyleBoxFlat CreateButtonStyle()
    => new StyleBoxFlat()
      .WithBorderWidth(BorderWidth)
      .WithCornerRadius(CornerRadius);

  private StyleBoxFlat CreateCheckButtonStyle()
    => new();

  private StyleBoxFlat CreateColorPickerStyle()
    => new StyleBoxFlat()
      .WithCornerRadius(CornerRadius)
      .WithBorderWidth(BorderWidth);

  private StyleBoxFlat CreateTabStyle()
    => new StyleBoxFlat()
      .WithTopCornerRadius(CornerRadius)
      .WithHorizontalContentMargins(MarginMedium);

  private StyleBoxFlat CreateDefaultStyle()
    => new StyleBoxFlat()
      .WithTopCornerRadius(CornerRadius)
      .WithHorizontalContentMargins(MarginSmall);

  private StyleBoxFlat CreateDialogStyle()
    => new StyleBoxFlat()
      .WithBorderWidth(BorderWidth)
      .WithCornerRadius(CornerRadius)
      .WithContentMargins(MarginMedium);

  private StyleBoxFlat CreateLineEditStyle()
    => new StyleBoxFlat()
      .WithBorderWidth(BorderWidth)
      .WithCornerRadius(CornerRadius)
      .WithContentMargins(MarginSmall);

  private StyleBoxFlat CreateItemListStyle()
    => new StyleBoxFlat()
      .WithBorderWidth(BorderWidth)
      .WithCornerRadius(CornerRadius);

  private StyleBoxFlat CreateItemSelectionStyle()
    => new();
}
