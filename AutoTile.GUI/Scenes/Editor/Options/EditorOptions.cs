using Godot;
using Qwaitumin.AutoTile.GUI.Core;
using Qwaitumin.AutoTile.GUI.Core.Signals;

namespace Qwaitumin.AutoTile.GUI.Scenes.Editor.Options;

public partial class EditorOptions : MarginContainer
{
  public Button SelectImageButton { private set; get; } = null!;
  public FileDialog SelectImageDialog { private set; get; } = null!;
  public Button SaveConfigurationButton { private set; get; } = null!;
  public FileDialog SaveConfigurationDialog { private set; get; } = null!;
  public FileDialog LoadConfigurationDialog { private set; get; } = null!;
  public OptionButton ToolsOptionsButton { private set; get; } = null!;
  public Button ClearConfigurationButton { private set; get; } = null!;
  public Button LoadConfigurationButton { private set; get; } = null!;

  public readonly ObservableVariable<Rect2I> ImageRectangleObservable = new(new());
  public readonly ObservableVariable<Texture2D> ImageTextureObservable = new(new());
  public readonly ObserverNotifier<EditorTools> ToolChangedNotifier = new();
  public readonly ObserverNotifier<bool> ClearConfigurationNotifier = new();
  public readonly ObserverNotifier<string> SaveConfigurationNotifier = new();
  public readonly ObserverNotifier<string> LoadConfigurationNotifier = new();
  public readonly ObservableVariable<string> ImageFileObservable = new("");

  public override void _Ready()
  {
    SelectImageButton = GetNode<Button>("V/Image/SelectImage");
    SelectImageDialog = GetNode<FileDialog>("V/Image/ImageDialog");
    ToolsOptionsButton = GetNode<OptionButton>("V/OptionButton");
    SaveConfigurationButton = GetNode<Button>("V/Configuration/SaveConfiguration");
    SaveConfigurationDialog = GetNode<FileDialog>("V/Configuration/SaveConfigurationDialog");
    ClearConfigurationButton = GetNode<Button>("V/Configuration/ClearConfiguration");
    LoadConfigurationButton = GetNode<Button>("V/Configuration/LoadConfiguration");
    LoadConfigurationDialog = GetNode<FileDialog>("V/Configuration/LoadConfigurationDialog");

    SelectImageDialog.FileSelected += LoadImageFromFile;
    SelectImageButton.Pressed += ShowImageDialog;
    ToolsOptionsButton.ItemSelected += ToolChanged;
    SaveConfigurationButton.Pressed += ShowSaveConfigurationDialog;
    SaveConfigurationDialog.FileSelected += SaveConfiguration;
    ClearConfigurationButton.Pressed += ClearConfiguration;
    LoadConfigurationDialog.FileSelected += LoadConfiguration;
    LoadConfigurationButton.Pressed += ShowLoadConfigurationDialog;
  }

  private void ToolChanged(long index)
  {
    string toolEnumString = ToolsOptionsButton.GetItemText((int)index);
    var enumValue = InputSanitizer.SanitizeEnum<EditorTools>(toolEnumString);
    ToolChangedNotifier.NotifyObservers(enumValue);
  }

  private void ClearConfiguration()
    => ClearConfigurationNotifier.NotifyObservers(true);

  private void SaveConfiguration(string filePath)
    => SaveConfigurationNotifier.NotifyObservers(filePath);

  private void LoadConfiguration(string filePath)
    => LoadConfigurationNotifier.NotifyObservers(filePath);

  private void ShowImageDialog()
    => SelectImageDialog.Show();

  private void ShowSaveConfigurationDialog()
    => SaveConfigurationDialog.Show();

  private void ShowLoadConfigurationDialog()
    => LoadConfigurationDialog.Show();

  private void LoadImageFromFile(string path)
  {
    var image = Image.LoadFromFile(path);
    image.Resize(
      image.GetWidth() * Editor.IMAGE_SCALING,
      image.GetHeight() * Editor.IMAGE_SCALING,
      Image.Interpolation.Nearest);

    var texture = ImageTexture.CreateFromImage(image);
    ImageTextureObservable.ChangeValueAndNotifyObservers(texture);

    Rect2I imageSize = new(Vector2I.Zero, new(image.GetWidth(), image.GetHeight()));
    ImageRectangleObservable.ChangeValueAndNotifyObservers(imageSize);
    ImageFileObservable.ChangeValueAndNotifyObservers(path);
    Editor.Logger.Log($"Changed image to: {path}");
  }
}
