using System.IO;
using Godot;
using Qwaitumin.LibreAutoTile.GUI;
using Qwaitumin.LibreAutoTile.GUI.GodotBindings;
using Qwaitumin.LibreAutoTile.GUI.Signals;

namespace Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.Options;

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
  public Control ImageUiContainer { private set; get; } = null!;
  public Control ConfigurationUiContainer { private set; get; } = null!;

  public readonly ObservableVariable<Rect2I> ImageRectangleObservable = new(new());
  public readonly ObservableVariable<Texture2D> ImageTextureObservable = new(new());
  public readonly EventNotifier<EditorTools> ToolHasChanged = new();
  public readonly EventNotifier<bool> ConfigurationCleared = new();
  public readonly EventNotifier<string> ConfigurationSaved = new();
  public readonly EventNotifier<string> ConfigurationLoaded = new();
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
    ImageUiContainer = GetNode<Control>("V/Image");
    ConfigurationUiContainer = GetNode<Control>("V/Configuration");

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
    ToolHasChanged.NotifyObservers(enumValue);
  }

  private void ClearConfiguration()
    => ConfigurationCleared.NotifyObservers(true);

  private void SaveConfiguration(string filePath)
    => ConfigurationSaved.NotifyObservers(filePath);

  private void LoadConfiguration(string filePath)
    => ConfigurationLoaded.NotifyObservers(filePath);

  private void ShowImageDialog()
    => SelectImageDialog.Show();

  private void ShowSaveConfigurationDialog()
    => SaveConfigurationDialog.Show();

  private void ShowLoadConfigurationDialog()
    => LoadConfigurationDialog.Show();

  private void LoadImageFromFile(string path)
  {
    var relativePath = Path.GetRelativePath(".", path);
    var image = Image.LoadFromFile(relativePath);
    image.Resize(
      image.GetWidth() * Editor.IMAGE_SCALING,
      image.GetHeight() * Editor.IMAGE_SCALING,
      Image.Interpolation.Nearest);

    var texture = ImageTexture.CreateFromImage(image);
    ImageTextureObservable.ChangeValueAndNotifyObservers(texture);

    Rect2I imageSize = new(Vector2I.Zero, new(image.GetWidth(), image.GetHeight()));
    ImageRectangleObservable.ChangeValueAndNotifyObservers(imageSize);
    ImageFileObservable.ChangeValueAndNotifyObservers(relativePath);
    GodotLogger.Logger.Log($"Changed image to: {relativePath}");
  }
}
