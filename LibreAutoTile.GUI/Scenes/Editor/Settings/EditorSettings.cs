using Godot;
using Qwaitumin.LibreAutoTile.GUI.Core;
using Qwaitumin.LibreAutoTile.GUI.Core.Signals;

namespace Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.Settings;

public partial class EditorSettings : Control, IState
{
  public ColorPickerButton SelectionColorPickerButton { private set; get; } = null!;
  public ColorPickerButton GridColorPickerButton { private set; get; } = null!;
  public ColorPickerButton BackgroundColorPickerButton { private set; get; } = null!;
  public LineEdit TileSizeLineEdit { private set; get; } = null!;

  public readonly ObservableVariable<Color> SelectionColorObservable = new(Colors.White);
  public readonly ObservableVariable<Color> GridColorObservable = new(Colors.Orange);
  public readonly ObservableVariable<Color> BackgroundColorObservable = new(Colors.DarkSlateGray);
  public readonly ObservableVariable<int> TileSizeObservable = new(16);
  public readonly ObservableVariable<int> ScaledTileSizeObservable = new(1);

  public override void _Ready()
  {
    SelectionColorPickerButton = GetNode<ColorPickerButton>("V/SelectionColor/ColorPickerButton");
    GridColorPickerButton = GetNode<ColorPickerButton>("V/GridColor/ColorPickerButton");
    BackgroundColorPickerButton = GetNode<ColorPickerButton>("V/BackgroundColor/ColorPickerButton");
    TileSizeLineEdit = GetNode<LineEdit>("V/TileSize/LineEdit");

    SelectionColorPickerButton.ColorChanged += ChangeSelectionColor;
    GridColorPickerButton.ColorChanged += ChangeGridColor;
    BackgroundColorPickerButton.ColorChanged += ChangeBackgroundColor;
    TileSizeLineEdit.TextSubmitted += ChangeTileSize;

    BackgroundColorObservable.AddObserver(RenderingServer.SetDefaultClearColor);
    TileSizeObservable.AddObserver(UpdateScaledTileSize);

    SelectionColorPickerButton.Color = SelectionColorObservable.Value;
    GridColorPickerButton.Color = GridColorObservable.Value;
    BackgroundColorPickerButton.Color = BackgroundColorObservable.Value;
    TileSizeLineEdit.Text = TileSizeObservable.Value.ToString();

    BackgroundColorObservable.NotifyObservers();
  }

  private void UpdateScaledTileSize(int tileSize)
    => ScaledTileSizeObservable.ChangeValueAndNotifyObservers(tileSize * Editor.IMAGE_SCALING);

  private void ChangeTileSize(string text)
  {
    var tileSize = InputSanitizer.SanitizeInt(text);
    if (tileSize > 0)
      TileSizeObservable.ChangeValueAndNotifyObservers(tileSize);
    TileSizeLineEdit.Text = TileSizeObservable.Value.ToString();
  }

  private void ChangeBackgroundColor(Color color)
    => BackgroundColorObservable.ChangeValueAndNotifyObservers(color);

  private void ChangeSelectionColor(Color color)
    => SelectionColorObservable.ChangeValueAndNotifyObservers(color);

  private void ChangeGridColor(Color color)
    => GridColorObservable.ChangeValueAndNotifyObservers(color);

  public void InitializeState()
    => Show();

  public void EndState()
    => Hide();
}