using Godot;
using Qwaitumin.LibreAutoTile.Configuration.Models;

namespace Qwaitumin.LibreAutoTile.GUI.Views;

public partial class SettingsPanel : MarginContainer
{
  public readonly LineEdit TileSizeLineEdit;
  public readonly LineEdit FontSizeLineEdit;
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
  public event Action<long>? ResolutionSelected;
  public event Action<TileShape>? TileShapeSelected;

  public SettingsPanel()
  {
    this.ExpandFill();

    var vbox = this.AppendVBox().ExpandFill();

    TileSizeLineEdit = vbox.AppendHBox().ExpandHorizontal()
      .AppendLabel("Tile size").ExpandHorizontal().ExpandVertical().FitContent().Back()
      .AppendLineEdit().ExpandHorizontal();

    FontSizeLineEdit = vbox.AppendHBox().ExpandHorizontal()
      .AppendLabel("Font size").ExpandHorizontal().ExpandVertical().FitContent().Back()
      .AppendLineEdit().ExpandHorizontal();

    ResolutionOptionButton = vbox.AppendHBox().ExpandHorizontal()
      .AppendLabel("Resolution").ExpandHorizontal().ExpandVertical().FitContent().Back()
      .AppendOptionButton().ExpandHorizontal();
    foreach (var resolution in Settings.RESOLUTIONS)
      ResolutionOptionButton.AddItem($"{resolution.X}x{resolution.Y}");

    TileShapeOptionButton = vbox.AppendHBox().ExpandHorizontal()
      .AppendLabel("Tile shape").ExpandHorizontal().ExpandVertical().FitContent().Back()
      .AppendOptionButton().ExpandHorizontal();
    foreach (var tileShape in Enum.GetValues<TileShape>())
      TileShapeOptionButton.AddItem(tileShape.ToString());

    GuiColorPicker = vbox.AppendHBox().ExpandHorizontal()
      .AppendLabel("GUI").ExpandHorizontal().ExpandVertical().FitContent().Back()
      .AppendColorPicker().ExpandHorizontal();

    SelectionColorPicker = vbox.AppendHBox().ExpandHorizontal()
      .AppendLabel("Selection").ExpandHorizontal().ExpandVertical().FitContent().Back()
      .AppendColorPicker().ExpandHorizontal();

    BackgroundColorPicker = vbox.AppendHBox().ExpandHorizontal()
      .AppendLabel("Background").ExpandHorizontal().ExpandVertical().FitContent().Back()
      .AppendColorPicker().ExpandHorizontal();

    GridColorPicker = vbox.AppendHBox().ExpandHorizontal()
      .AppendLabel("Grid").ExpandHorizontal().ExpandVertical().FitContent().Back()
      .AppendColorPicker().ExpandHorizontal();

    ProbabilityColorPicker = vbox.AppendHBox().ExpandHorizontal()
      .AppendLabel("Probability").ExpandHorizontal().ExpandVertical().FitContent().Back()
      .AppendColorPicker().ExpandHorizontal();

    GridColorPicker.ColorChanged += color => GridColorChanged?.Invoke(color);
    SelectionColorPicker.ColorChanged += color => SelectionColorChanged?.Invoke(color);
    GuiColorPicker.ColorChanged += color => GuiColorChanged?.Invoke(color);
    BackgroundColorPicker.ColorChanged += color => BackgroundColorChanged?.Invoke(color);
    ProbabilityColorPicker.ColorChanged += color => ProbabilityColorChanged?.Invoke(color);
    TileSizeLineEdit.TextSubmitted += text => TileSizeSubmitted?.Invoke(text);
    FontSizeLineEdit.TextSubmitted += text => FontSizeSubmitted?.Invoke(text);
    ResolutionOptionButton.ItemSelected += index => ResolutionSelected?.Invoke(index);
    TileShapeOptionButton.ItemSelected += index =>
      TileShapeSelected?.Invoke(Enum.GetValues<TileShape>()[(int)index]);
  }

  public void SetTileSizeText(string text)
    => TileSizeLineEdit.Text = text;

  public void SetFontSizeText(string text)
    => FontSizeLineEdit.Text = text;

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

  public void SelectResolution(long index)
    => ResolutionOptionButton.Select((int)index);

  public void SelectTileShape(TileShape tileShape)
    => TileShapeOptionButton.Select(Array.IndexOf(Enum.GetValues<TileShape>(), tileShape));
}
