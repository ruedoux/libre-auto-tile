using Godot;
using LibreAutoTile.Configuration.Models;
using LibreAutoTile.GUI.Editor.Model;
using LibreAutoTile.GUI.Settings;

namespace LibreAutoTile.GUI.Editor.View.Tools;

public partial class SettingsPanel : MarginContainer
{
  public readonly LineEdit TileSizeLineEdit;
  public readonly LineEdit FontSizeLineEdit;
  public readonly LineEdit ProjectRootLineEdit;
  public readonly Button ProjectRootButton;
  public readonly FileDialog ProjectRootDialog;
  public readonly OptionButton ResolutionOptionButton;
  public readonly OptionButton TileShapeOptionButton;
  public readonly ColorPickerButton GuiColorPicker;
  public readonly ColorPickerButton SelectionColorPicker;
  public readonly ColorPickerButton BackgroundColorPicker;
  public readonly ColorPickerButton GridColorPicker;
  public readonly ColorPickerButton ProbabilityColorPicker;

  public event Action<Color>? GridColorChanged;
  public event Action<Color>? SelectionColorChanged;
  public event Action<Color>? GuiColorChanged;
  public event Action<Color>? BackgroundColorChanged;
  public event Action<Color>? ProbabilityColorChanged;
  public event Action<string>? TileSizeSubmitted;
  public event Action<string>? FontSizeSubmitted;
  public event Action<Vector2I, bool>? ResolutionSelected;
  public event Action<TileShape>? TileShapeSelected;
  public event Action<string>? ProjectRootSelected;

  private readonly AppSettings settings;

  public SettingsPanel(AppSettings settings)
  {
    this.settings = settings;
    this.ExpandFill();

    var vbox = this.AppendVBox().ExpandFill();

    TileSizeLineEdit = vbox.AppendHBox().ExpandHorizontal()
      .AppendLabel("Tile size").ExpandFill().FitContent().DisableAutowrap().Back()
      .AppendLineEdit().ExpandHorizontal();

    FontSizeLineEdit = vbox.AppendHBox().ExpandHorizontal()
      .AppendLabel("Font size").ExpandFill().FitContent().DisableAutowrap().Back()
      .AppendLineEdit().ExpandHorizontal();

    var projectRootRow = vbox.AppendHBox().ExpandHorizontal();
    projectRootRow.AppendLabel("Project root").ExpandFill().FitContent().DisableAutowrap();
    ProjectRootLineEdit = projectRootRow.AppendLineEdit().WithPlaceholder("(GUI folder)").ExpandHorizontal();
    ProjectRootButton = projectRootRow.AppendButton("Browse").ExpandHorizontal();

    ProjectRootDialog = this.AppendFileDialog("Select Project Root Folder")
      .WithFileMode(FileDialog.FileModeEnum.OpenDir);

    ResolutionOptionButton = vbox.AppendHBox().ExpandHorizontal()
      .AppendLabel("Resolution").ExpandFill().DisableAutowrap().Back()
      .AppendOptionButton().ExpandHorizontal();

    TileShapeOptionButton = vbox.AppendHBox().ExpandHorizontal()
      .AppendLabel("Tile shape").ExpandFill().FitContent().DisableAutowrap().Back()
      .AppendOptionButton().ExpandHorizontal();
    foreach (var tileShape in Enum.GetValues<TileShape>())
      TileShapeOptionButton.AddItem(tileShape.ToString());

    GuiColorPicker = vbox.AppendHBox().ExpandHorizontal()
      .AppendLabel("GUI").ExpandFill().FitContent().DisableAutowrap().Back()
      .AppendColorPicker(settings.UiSettings).ExpandHorizontal();

    SelectionColorPicker = vbox.AppendHBox().ExpandHorizontal()
      .AppendLabel("Selection").ExpandFill().FitContent().DisableAutowrap().Back()
      .AppendColorPicker(settings.UiSettings).ExpandHorizontal();

    BackgroundColorPicker = vbox.AppendHBox().ExpandHorizontal()
      .AppendLabel("Background").ExpandFill().FitContent().DisableAutowrap().Back()
      .AppendColorPicker(settings.UiSettings).ExpandHorizontal();

    GridColorPicker = vbox.AppendHBox().ExpandHorizontal()
      .AppendLabel("Grid").ExpandFill().FitContent().DisableAutowrap().Back()
      .AppendColorPicker(settings.UiSettings).ExpandHorizontal();

    ProbabilityColorPicker = vbox.AppendHBox().ExpandHorizontal()
      .AppendLabel("Probability").ExpandFill().FitContent().DisableAutowrap().Back()
      .AppendColorPicker(settings.UiSettings).ExpandHorizontal();

    GridColorPicker.ColorChanged += color => GridColorChanged?.Invoke(color);
    SelectionColorPicker.ColorChanged += color => SelectionColorChanged?.Invoke(color);
    GuiColorPicker.ColorChanged += color => GuiColorChanged?.Invoke(color);
    BackgroundColorPicker.ColorChanged += color => BackgroundColorChanged?.Invoke(color);
    ProbabilityColorPicker.ColorChanged += color => ProbabilityColorChanged?.Invoke(color);
    TileSizeLineEdit.TextSubmitted += text => TileSizeSubmitted?.Invoke(text);
    FontSizeLineEdit.TextSubmitted += text => FontSizeSubmitted?.Invoke(text);
    ResolutionOptionButton.ItemSelected += index =>
      ResolutionSelected?.Invoke(
        ResolutionOptionButton.GetItemMetadata((int)index).AsVector2I(), (int)index == 0);
    TileShapeOptionButton.ItemSelected += index =>
      TileShapeSelected?.Invoke(Enum.GetValues<TileShape>()[(int)index]);
    ProjectRootButton.Pressed += () =>
      ProjectRootDialog.PopupCenteredRatio(settings.UiSettings.DialogScreenRatio);
    ProjectRootLineEdit.TextSubmitted += text => ProjectRootSelected?.Invoke(text);
    ProjectRootDialog.DirSelected += dir => ProjectRootSelected?.Invoke(dir);
  }

  public void SetTileSizeText(string text)
    => TileSizeLineEdit.Text = text;

  public void SetFontSizeText(string text)
    => FontSizeLineEdit.Text = text;

  public void SetProjectRootText(string text)
    => ProjectRootLineEdit.Text = text;

  public void SetGuiColor(Color color)
    => GuiColorPicker.Color = color;

  public void SetSelectionColor(Color color)
    => SelectionColorPicker.Color = color;

  public void SetBackgroundColor(Color color)
    => BackgroundColorPicker.Color = color;

  public void SetGridColor(Color color)
    => GridColorPicker.Color = color;

  public void SetProbabilityColor(Color color)
    => ProbabilityColorPicker.Color = color;

  public void SelectResolution(DisplayOptions options)
    => PopulateResolutions(options);

  private void PopulateResolutions(DisplayOptions options)
  {
    ResolutionOptionButton.Clear();

    var native = DisplayServer.ScreenGetSize(DisplayServer.WindowGetCurrentScreen());
    AddResolutionItem(0, "Native", native);

    var current = new Vector2I(options.Width, options.Height);
    var sizes = UiSettings.STANDARD_RESOLUTIONS
      .Where(size => size != native)
      .ToList();
    if (!options.Native && current != native && !sizes.Contains(current))
      sizes.Add(current);
    sizes.Sort((a, b) => (b.X * b.Y).CompareTo(a.X * a.Y));

    var selectedIndex = options.Native ? 0 : -1;
    for (var i = 0; i < sizes.Count; i++)
    {
      var index = i + 1;
      AddResolutionItem(index, $"{sizes[i].X} x {sizes[i].Y}", sizes[i]);
      if (!options.Native && sizes[i] == current)
        selectedIndex = index;
    }

    ResolutionOptionButton.Selected = selectedIndex >= 0 ? selectedIndex : 0;
  }

  private void AddResolutionItem(int index, string label, Vector2I size)
  {
    ResolutionOptionButton.AddItem(label);
    ResolutionOptionButton.SetItemMetadata(index, size);
  }

  public void SelectTileShape(TileShape tileShape)
    => TileShapeOptionButton.Select(Array.IndexOf(Enum.GetValues<TileShape>(), tileShape));

  public void ShowProjectRootDialog()
    => ProjectRootDialog.PopupCenteredRatio(settings.UiSettings.DialogScreenRatio);
}
