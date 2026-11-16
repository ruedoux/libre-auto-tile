using Godot;
using LibreAutoTile.GUI.Settings;

namespace LibreAutoTile.GUI;

public static class GodotExtensions
{
  public static bool IsMouseOnElement(Control control)
    => control.GetGlobalRect().HasPoint(control.GetGlobalMousePosition());

  public static bool IsMouseOnElements(Control[] controls)
  {
    foreach (var control in controls)
      if (IsMouseOnElement(control)) return true;
    return false;
  }

  public static T AppendChild<T>(this Node parent, T child) where T : Node
  {
    parent.AddChild(child);
    return child;
  }

  public static T ExpandFill<T>(this T control) where T : Control
  {
    control.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
    control.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
    return control;
  }

  public static T ExpandHorizontal<T>(this T control) where T : Control
  {
    control.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
    return control;
  }

  public static T ExpandVertical<T>(this T control) where T : Control
  {
    control.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
    return control;
  }

  public static T WithDefaultTheme<T>(this T node, UiSettings settings) where T : Node
  {
    switch (node)
    {
      case Control control:
        control.Theme = settings.Theme;
        break;
      case Window window:
        window.Theme = settings.Theme;
        break;
    }
    return node;
  }

  public static HBoxContainer AppendHBox(this Node parent)
  {
    var box = new HBoxContainer();
    parent.AddChild(box);
    return box;
  }

  public static VBoxContainer AppendVBox(this Node parent)
  {
    var box = new VBoxContainer();
    parent.AddChild(box);
    return box;
  }

  public static RichTextLabel AppendLabel(this Node parent, string text)
  {
    var label = new RichTextLabel
    {
      Text = text
    };
    label.WithAlignment(VerticalAlignment.Center);
    label.AutowrapMode = TextServer.AutowrapMode.Off;
    parent.AddChild(label);
    return label;
  }

  public static ScrollContainer AppendScroll(this Node parent)
  {
    var scroll = new ScrollContainer();
    parent.AddChild(scroll);
    return scroll;
  }

  public static CenterContainer AppendCenterContainer(this Node parent)
  {
    var center = new CenterContainer();
    parent.AddChild(center);
    return center;
  }

  public static GridContainer AppendGridContainer(this Node parent)
  {
    var grid = new GridContainer();
    parent.AddChild(grid);
    return grid;
  }

  public static SplitContainer AppendSplitContainer(this Node parent)
  {
    var split = new SplitContainer();
    parent.AddChild(split);
    return split;
  }

  public static PanelContainer AppendPanelContainer(this Node parent)
  {
    var panel = new PanelContainer();
    parent.AddChild(panel);
    return panel;
  }

  public static CanvasLayer AppendCanvasLayer(this Node parent)
  {
    var layer = new CanvasLayer();
    parent.AddChild(layer);
    return layer;
  }

  public static Node Back(this Node node) => node.GetParent();

  public static T FitContent<T>(this T label, bool fit = true) where T : RichTextLabel
  {
    label.FitContent = fit;
    return label;
  }

  public static T DisableAutowrap<T>(this T label) where T : RichTextLabel
  {
    label.AutowrapMode = TextServer.AutowrapMode.Off;
    return label;
  }

  public static T WithAlignment<T>(this T label, HorizontalAlignment alignment) where T : RichTextLabel
  {
    label.HorizontalAlignment = alignment;
    return label;
  }

  public static T WithAlignment<T>(this T label, VerticalAlignment alignment) where T : RichTextLabel
  {
    label.VerticalAlignment = alignment;
    return label;
  }

  public static LineEdit AppendLineEdit(this Node parent, string content = "")
  {
    var widget = new LineEdit
    {
      Text = content
    };
    parent.AddChild(widget);
    return widget;
  }

  public static LineEdit WithPlaceholder(this LineEdit lineEdit, string placeholder)
  {
    lineEdit.PlaceholderText = placeholder;
    return lineEdit;
  }

  public static ColorPickerButton AppendColorPicker(this Node parent, UiSettings settings)
  {
    var widget = new ColorPickerButton();
    widget.GetPopup().Theme = settings.Theme;
    parent.AddChild(widget);
    return widget;
  }

  public static SpinBox AppendSpinBox(
    this Node parent,
    double min = 0,
    double max = double.PositiveInfinity,
    double step = 1)
  {
    var widget = new SpinBox
    {
      MinValue = min,
      MaxValue = max,
      Step = step
    };
    parent.AddChild(widget);
    return widget;
  }

  public static FileDialog AppendFileDialog(
    this Node parent,
    string title,
    FileDialog.AccessEnum access = FileDialog.AccessEnum.Filesystem)
  {
    var dialog = new FileDialog
    {
      Title = title,
      ModeOverridesTitle = false,
      Access = access,
      HiddenFilesToggleEnabled = false,
      FavoritesEnabled = false,
      FileSortOptionsEnabled = false,
      ShowHiddenFiles = true,
    };
    parent.AddChild(dialog);
    return dialog;
  }

  public static FileDialog WithFileMode(this FileDialog fileDialog, FileDialog.FileModeEnum fileMode)
  {
    fileDialog.FileMode = fileMode;
    return fileDialog;
  }

  public static FileDialog WithFilters(this FileDialog fileDialog, string[] filters)
  {
    fileDialog.Filters = filters;
    return fileDialog;
  }

  public static Button AppendButton(this Node parent, string text)
  {
    var widget = new Button
    {
      Text = text
    };
    parent.AddChild(widget);
    return widget;
  }

  public static CheckButton AppendCheckButton(this Node parent)
  {
    var widget = new CheckButton
    {
      Alignment = HorizontalAlignment.Center
    };
    parent.AddChild(widget);
    return widget;
  }

  public static OptionButton AppendOptionButton(this Node parent)
  {
    var widget = new OptionButton();
    parent.AddChild(widget);
    return widget;
  }

  public static TabContainer AppendTabContainer(this Node parent)
  {
    var widget = new TabContainer();
    parent.AddChild(widget);
    return widget;
  }

  public static T AddTab<T>(this TabContainer tabs, T child, string title) where T : Control
  {
    tabs.AddChild(child);
    tabs.SetTabTitle(tabs.GetTabCount() - 1, title);
    return child;
  }

  public static T FullRect<T>(this T control) where T : Control
  {
    control.SetAnchorsPreset(Control.LayoutPreset.FullRect);
    return control;
  }

  public static T WithMinSize<T>(this T control, float width, float height) where T : Control
  {
    control.CustomMinimumSize = new Vector2(width, height);
    return control;
  }

  public static T WithMargins<T>(this T control, int all) where T : Control
  {
    control.AddThemeConstantOverride("margin_left", all);
    control.AddThemeConstantOverride("margin_right", all);
    control.AddThemeConstantOverride("margin_top", all);
    control.AddThemeConstantOverride("margin_bottom", all);
    return control;
  }

  public static T WithMargins<T>(this T control, int left, int right, int top, int bottom) where T : Control
  {
    control.AddThemeConstantOverride("margin_left", left);
    control.AddThemeConstantOverride("margin_right", right);
    control.AddThemeConstantOverride("margin_top", top);
    control.AddThemeConstantOverride("margin_bottom", bottom);
    return control;
  }

  public static StyleBoxFlat WithBorderWidth(this StyleBoxFlat styleBox, int width)
  {
    styleBox.SetBorderWidthAll(width);
    return styleBox;
  }

  public static StyleBoxFlat WithCornerRadius(this StyleBoxFlat styleBox, int radius)
  {
    styleBox.SetCornerRadiusAll(radius);
    return styleBox;
  }

  public static StyleBoxFlat WithTopCornerRadius(this StyleBoxFlat styleBox, int radius)
  {
    styleBox.SetCornerRadius(Corner.TopLeft, radius);
    styleBox.SetCornerRadius(Corner.TopRight, radius);
    return styleBox;
  }

  public static StyleBoxFlat WithContentMargins(this StyleBoxFlat styleBox, int all)
  {
    styleBox.ContentMarginLeft = all;
    styleBox.ContentMarginRight = all;
    styleBox.ContentMarginTop = all;
    styleBox.ContentMarginBottom = all;
    return styleBox;
  }

  public static StyleBoxFlat WithHorizontalContentMargins(this StyleBoxFlat styleBox, int all)
  {
    styleBox.ContentMarginLeft = all;
    styleBox.ContentMarginRight = all;
    return styleBox;
  }

  public static T StretchRatio<T>(this T control, float ratio) where T : Control
  {
    control.SizeFlagsStretchRatio = ratio;
    return control;
  }

  public static MarginContainer AppendMargin(this Node parent)
  {
    var widget = new MarginContainer();
    parent.AddChild(widget);
    return widget;
  }
}
