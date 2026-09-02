using Godot;
using Qwaitumin.LibreAutoTile.GUI.Views.Presentation;

namespace Qwaitumin.LibreAutoTile.GUI.Views;

public partial class EditorTiles : MarginContainer
{
  public readonly Button AddTileButton;
  public readonly OptionButton ImageSelector;
  public readonly VBoxContainer TileList;

  private readonly HBoxContainer imageRow;
  private readonly List<string> imagePaths = [];
  private readonly Dictionary<object, GuiTile> rows = [];

  public event Action? AddTileRequested;
  public event Action<string>? ImageSelected;
  public event Action<TileViewModel>? RemoveRequested;
  public event Action<TileViewModel, string>? NameSubmitted;
  public event Action<TileViewModel, string>? IdSubmitted;
  public event Action<TileViewModel, string>? ConnectionGroupSubmitted;
  public event Action<TileViewModel>? SelectRequested;
  public event Action<TileViewModel>? MoveUpRequested;
  public event Action<TileViewModel>? MoveDownRequested;
  public event Action<TileViewModel, Color>? ColorChanged;

  public EditorTiles()
  {
    this.ExpandFill();

    var vbox = this.AppendVBox().ExpandFill();

    imageRow = vbox.AppendHBox().ExpandHorizontal();
    imageRow.AppendLabel("Image").ExpandHorizontal().ExpandVertical();
    ImageSelector = imageRow.AppendOptionButton().ExpandHorizontal().ExpandVertical();
    imageRow.Visible = false;
    AddTileButton = vbox.AppendButton("Add new tile").ExpandHorizontal();
    TileList = vbox.AppendScroll().ExpandHorizontal().ExpandVertical()
      .AppendVBox().ExpandHorizontal();

    AddTileButton.Pressed += () => AddTileRequested?.Invoke();
    ImageSelector.ItemSelected += idx =>
    {
      int i = (int)idx;
      if (i >= 0 && i < imagePaths.Count)
        ImageSelected?.Invoke(imagePaths[i]);
    };
  }

  public void RefreshImageOptions(IEnumerable<string> imageNames, string selectedImage)
  {
    imagePaths.Clear();
    ImageSelector.Clear();
    int selectedIndex = -1;
    int index = 0;
    foreach (var imageName in imageNames)
    {
      imagePaths.Add(imageName);
      ImageSelector.AddItem(Path.GetFileName(imageName), index);
      if (imageName == selectedImage)
        selectedIndex = index;
      index++;
    }
    ImageSelector.Select(selectedIndex);
    imageRow.Visible = ImageSelector.ItemCount > 0;
  }

  public void Render(IReadOnlyList<TileViewModel> tiles)
  {
    var newKeys = tiles.Select(tile => tile.Key).ToHashSet();
    foreach (var key in rows.Keys.Where(key => !newKeys.Contains(key)).ToList())
      RemoveRow(key);

    int index = 0;
    foreach (var viewModel in tiles)
    {
      if (!rows.TryGetValue(viewModel.Key, out var row))
      {
        row = new GuiTile(viewModel);
        rows[viewModel.Key] = row;
        WireRow(row, viewModel);
        TileList.AddChild(row);
      }

      TileList.MoveChild(row, index);
      row.SetActive(viewModel.IsActive);
      row.Refresh();
      index++;
    }
  }

  private void WireRow(GuiTile row, TileViewModel viewModel)
  {
    row.RemoveRequested += () => RemoveRequested?.Invoke(viewModel);
    row.NameSubmitted += name => NameSubmitted?.Invoke(viewModel, name);
    row.IdSubmitted += id => IdSubmitted?.Invoke(viewModel, id);
    row.ConnectionGroupSubmitted += group => ConnectionGroupSubmitted?.Invoke(viewModel, group);
    row.SelectRequested += () => SelectRequested?.Invoke(viewModel);
    row.MoveUpRequested += () => MoveUpRequested?.Invoke(viewModel);
    row.MoveDownRequested += () => MoveDownRequested?.Invoke(viewModel);
    row.ColorChanged += color => ColorChanged?.Invoke(viewModel, color);
  }

  private void RemoveRow(object key)
  {
    if (!rows.Remove(key, out var row))
      return;
    TileList.RemoveChild(row);
    row.QueueFree();
  }
}
