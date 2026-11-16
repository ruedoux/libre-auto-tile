using Godot;
using LibreAutoTile.GUI.Editor.Model;
using LibreAutoTile.GUI.Editor.View.Widgets;
using LibreAutoTile.GUI.Settings;

namespace LibreAutoTile.GUI.Editor.View.Tools;

public partial class TilesPanel : MarginContainer
{
  public readonly Button AddTileButton;
  public readonly OptionButton ImageSelector;
  public readonly VBoxContainer TileList;
  public readonly LineEdit WildcardLineEdit;

  private readonly AppSettings settings;
  private readonly HBoxContainer imageRow;
  private readonly List<string> imagePaths = [];
  private readonly Dictionary<TileModel, GuiTile> rows = [];

  public event Action? AddTileRequested;
  public event Action<string>? ImageSelected;
  public event Action<TileModel>? RemoveRequested;
  public event Action<TileModel, string>? NameSubmitted;
  public event Action<TileModel, string>? IdSubmitted;
  public event Action<TileModel, string>? ConnectionGroupSubmitted;
  public event Action<TileModel>? SelectRequested;
  public event Action<TileModel>? MoveUpRequested;
  public event Action<TileModel>? MoveDownRequested;
  public event Action<TileModel, Color>? ColorChanged;
  public event Action<string>? WildcardIdChanged;

  public TilesPanel(AppSettings settings)
  {
    this.settings = settings;
    this.ExpandFill();

    var vbox = this.AppendVBox().ExpandFill();
    imageRow = vbox.AppendHBox().ExpandHorizontal();
    ImageSelector = imageRow.AppendOptionButton().ExpandFill();
    imageRow.Visible = false;

    var wildcardRow = vbox.AppendHBox().ExpandHorizontal();
    wildcardRow.AppendLabel("Wildcard Id").ExpandHorizontal();
    WildcardLineEdit = wildcardRow.AppendLineEdit().ExpandHorizontal();
    AddTileButton = vbox.AppendButton("Add new tile").ExpandHorizontal();
    TileList = vbox.AppendScroll().ExpandFill()
      .AppendVBox().ExpandHorizontal();

    WildcardLineEdit.TextChanged += text => WildcardIdChanged?.Invoke(text);

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

  public void Render(IReadOnlyList<TileModel> tiles, TileModel? activeTile)
  {
    var newKeys = tiles.ToHashSet();
    foreach (var key in rows.Keys.Where(key => !newKeys.Contains(key)).ToList())
      RemoveRow(key);

    int index = 0;
    foreach (var tile in tiles)
    {
      if (!rows.TryGetValue(tile, out var row))
      {
        row = new GuiTile(tile, settings);
        rows[tile] = row;
        WireRow(row, tile);
        TileList.AddChild(row);
      }

      TileList.MoveChild(row, index);
      row.SetActive(tile == activeTile);
      row.Refresh(tile);
      index++;
    }
  }

  private void WireRow(GuiTile row, TileModel tile)
  {
    row.RemoveRequested += () => RemoveRequested?.Invoke(tile);
    row.NameSubmitted += name => NameSubmitted?.Invoke(tile, name);
    row.IdSubmitted += id => IdSubmitted?.Invoke(tile, id);
    row.ConnectionGroupSubmitted += group => ConnectionGroupSubmitted?.Invoke(tile, group);
    row.SelectRequested += () => SelectRequested?.Invoke(tile);
    row.MoveUpRequested += () => MoveUpRequested?.Invoke(tile);
    row.MoveDownRequested += () => MoveDownRequested?.Invoke(tile);
    row.ColorChanged += color => ColorChanged?.Invoke(tile, color);
  }

  private void RemoveRow(TileModel key)
  {
    if (!rows.Remove(key, out var row))
      return;
    TileList.RemoveChild(row);
    row.QueueFree();
  }

  public void SetWildcardIdText(string text) => WildcardLineEdit.Text = text;
}
