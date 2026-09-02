using Godot;

namespace Qwaitumin.LibreAutoTile.GUI.Views;


public static class Settings
{
  public static readonly Font FONT = ResourceLoader.Load<FontFile>("res://resources/SpaceMono-Regular.ttf");

  public const int IMAGE_SCALING = 4;
  public const int DEFAULT_FONT_SIZE = 32;

  public const float DIALOG_SCREEN_RATIO = 0.7f;
  public const int DIALOG_THUMBNAIL_SIZE = 128;

  public static readonly Vector2I[] RESOLUTIONS =
  [
    new(1280, 720),
    new(1600, 900),
    new(1920, 1080),
    new(2560, 1440),
    new(3840, 2160),
  ];
  public const int DEFAULT_RESOLUTION_INDEX = 2; // 1920x1080

  public static readonly int BORDER_WIDTH = 2;
  public static readonly int CORNER_RADIUS = 8;
  public static readonly int MARGIN_SMALL = 4;
  public static readonly int MARGIN_MEDIUM = 8;
  public static readonly int MARGIN_BIG = 16;

  private sealed record StyleDefinition(
    string ControlType, string State, StyleBoxFlat Style,
    Func<Color, Color> Background, Func<Color, Color>? Border = null);

  private static readonly StyleDefinition[] STYLES =
  [
    new("Button", "normal", CreateButtonStyle(), bg => bg, bg => bg.Lightened(0.25f)),
    new("Button", "hover", CreateButtonStyle(), bg => bg.Lightened(0.1f), bg => bg.Lightened(0.25f)),
    new("Button", "pressed", CreateButtonStyle(), bg => bg.Darkened(0.25f), bg => bg.Lightened(0.25f)),
    new("Button", "focus", CreateButtonStyle(), bg => bg, _ => new(1f, 1f, 1f)),
    new("Button", "disabled", CreateButtonStyle(), bg => bg.Darkened(0.25f), bg => bg.Lightened(0.25f)),
    new("ColorPickerButton", "normal", CreateColorPickerStyle(), bg => bg),
    new("ColorPickerButton", "hover", CreateColorPickerStyle(), bg => bg.Lightened(0.1f)),
    new("ColorPickerButton", "pressed", CreateColorPickerStyle(), bg => bg.Darkened(0.25f)),
    new("ColorPickerButton", "focus", CreateColorPickerStyle(), bg => bg),
    new("ColorPickerButton", "disabled", CreateColorPickerStyle(), bg => bg.Darkened(0.25f)),
    new("TabContainer", "tab_selected", CreateTabStyle(), bg => bg),
    new("TabContainer", "tab_unselected", CreateTabStyle(), bg => bg.Darkened(0.25f)),
    new("TabContainer", "tab_hovered", CreateTabStyle(), bg => bg.Lightened(0.1f)),
    new("TabContainer", "tab_disabled", CreateTabStyle(), bg => bg.Darkened(0.25f)),
    new("TabContainer", "panel", new StyleBoxFlat().WithContentMargins(MARGIN_MEDIUM), bg => bg),
    new("TabContainer", "tabbar_background", new(), bg => bg.Darkened(0.4f)),
    new("RichTextLabel", "normal", CreateDefaultStyle(), bg => bg, bg => bg.Lightened(0.25f)),
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
  ];

  private static readonly (string ControlType, string Name, int Value)[] CONSTANTS =
  [
    ("FileDialog", "thumbnail_size", DIALOG_THUMBNAIL_SIZE),
  ];

  public static void ApplyGuiColor(Color color)
    => RefreshStyles(color);

  public static void ApplyBackgroundColor(Color color)
    => RenderingServer.SetDefaultClearColor(color);

  public static void ApplyFontSize(int size)
    => GetDefaultTheme().DefaultFontSize = size;

  private static void RefreshStyles(Color background)
  {
    foreach (var style in STYLES)
    {
      style.Style.BgColor = style.Background(background);
      if (style.Border is not null)
        style.Style.BorderColor = style.Border(background);
    }
  }

  private static Theme? theme;

  public static Theme GetDefaultTheme()
  {
    if (theme is not null)
      return theme;

    theme = new Theme
    {
      DefaultFont = FONT,
      DefaultFontSize = DEFAULT_FONT_SIZE
    };
    foreach (var style in STYLES)
      theme.SetStylebox(style.State, style.ControlType, style.Style);
    foreach (var (controlType, name, value) in CONSTANTS)
      theme.SetConstant(name, controlType, value);
    return theme;
  }

  private static StyleBoxFlat CreateButtonStyle()
    => new StyleBoxFlat()
      .WithBorderWidth(BORDER_WIDTH)
      .WithCornerRadius(CORNER_RADIUS);

  private static StyleBoxFlat CreateColorPickerStyle()
    => new StyleBoxFlat()
      .WithCornerRadius(CORNER_RADIUS);

  private static StyleBoxFlat CreateTabStyle()
    => new StyleBoxFlat()
      .WithTopCornerRadius(CORNER_RADIUS)
      .WithHorizontalContentMargins(MARGIN_MEDIUM);

  private static StyleBoxFlat CreateDefaultStyle()
    => new StyleBoxFlat()
      .WithTopCornerRadius(CORNER_RADIUS)
      .WithHorizontalContentMargins(MARGIN_SMALL);

  private static StyleBoxFlat CreateDialogStyle()
    => new StyleBoxFlat()
      .WithBorderWidth(BORDER_WIDTH)
      .WithCornerRadius(CORNER_RADIUS)
      .WithContentMargins(MARGIN_MEDIUM);

  private static StyleBoxFlat CreateLineEditStyle()
    => new StyleBoxFlat()
      .WithBorderWidth(BORDER_WIDTH)
      .WithCornerRadius(CORNER_RADIUS)
      .WithContentMargins(MARGIN_SMALL);

  private static StyleBoxFlat CreateItemListStyle()
    => new StyleBoxFlat()
      .WithBorderWidth(BORDER_WIDTH)
      .WithCornerRadius(CORNER_RADIUS);

  private static StyleBoxFlat CreateItemSelectionStyle()
    => new();
}
