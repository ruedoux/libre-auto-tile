using Godot;
using Qwaitumin.LibreAutoTile.GUI.Views.Presentation;

namespace Qwaitumin.LibreAutoTile.GUI.Controllers;

public class TileController
{
  private readonly EditorContext context;

  public event Action<int>? TileDeleted;
  public event Action<int, int>? TileIdChanged;

  public TileController(EditorContext context)
  {
    this.context = context;
    var view = context.EditorScene;
    view.TilesPanel.AddTileRequested += OnAddTileRequested;
    view.TilesPanel.RemoveRequested += OnTileRemoveRequested;
    view.TilesPanel.NameSubmitted += OnTileNameSubmitted;
    view.TilesPanel.IdSubmitted += OnTileIdSubmitted;
    view.TilesPanel.ConnectionGroupSubmitted += OnTileConnectionGroupSubmitted;
    view.TilesPanel.SelectRequested += OnTileSelectRequested;
    view.TilesPanel.MoveUpRequested += OnTileMoveUpRequested;
    view.TilesPanel.MoveDownRequested += OnTileMoveDownRequested;
    view.TilesPanel.ColorChanged += OnTileColorChanged;
    view.TilesPanel.WildcardIdChanged += OnWildcardIdChanged;
  }

  private void OnAddTileRequested()
  {
    var tile = context.EditorData.Tiles.AddNew();
    context.RefreshTilesView();
    GodotLogger.LOGGER.Log($"Added new tile: {tile.TileName}");
  }

  private void OnTileRemoveRequested(TileViewModel viewModel)
  {
    var tile = TileViewMapper.ToModel(viewModel);
    TileDeleted?.Invoke(tile.TileId);
    context.EditorData.Tiles.Remove(tile);
    context.RefreshTilesView();
    GodotLogger.LOGGER.Log($"Removed tile {tile.TileName}");
  }

  private void OnTileNameSubmitted(TileViewModel viewModel, string name)
  {
    context.EditorData.Tiles.TryChangeName(TileViewMapper.ToModel(viewModel), name);
    context.RefreshTilesView();
  }

  private void OnTileIdSubmitted(TileViewModel viewModel, string text)
  {
    var tile = TileViewMapper.ToModel(viewModel);
    int oldId = tile.TileId;
    context.EditorData.Tiles.TryChangeId(tile, text);
    if (tile.TileId != oldId)
      TileIdChanged?.Invoke(tile.TileId, oldId);
    context.RefreshTilesView();
  }

  private void OnTileConnectionGroupSubmitted(TileViewModel viewModel, string text)
  {
    context.EditorData.Tiles.TryChangeConnectionGroup(TileViewMapper.ToModel(viewModel), text);
    context.RefreshTilesView();
  }

  private void OnTileSelectRequested(TileViewModel viewModel)
  {
    var tile = TileViewMapper.ToModel(viewModel);
    context.EditorData.Tiles.SetActive(tile);
    context.RefreshTilesView();
    context.RedrawBitmask();
  }

  private void OnTileMoveUpRequested(TileViewModel viewModel)
  {
    context.EditorData.Tiles.MoveUp(TileViewMapper.ToModel(viewModel));
    context.RefreshTilesView();
  }

  private void OnTileMoveDownRequested(TileViewModel viewModel)
  {
    context.EditorData.Tiles.MoveDown(TileViewMapper.ToModel(viewModel));
    context.RefreshTilesView();
  }

  private void OnTileColorChanged(TileViewModel viewModel, Color color)
  {
    TileViewMapper.ToModel(viewModel).Color = color;
    context.RedrawBitmask();
  }

  private void OnWildcardIdChanged(string text)
  {
    if (string.IsNullOrWhiteSpace(text))
    {
      context.EditorData.WildcardId = null;
      return;
    }

    if (int.TryParse(text, out int wildcardId) && wildcardId >= 0)
      context.EditorData.WildcardId = wildcardId;
  }
}
