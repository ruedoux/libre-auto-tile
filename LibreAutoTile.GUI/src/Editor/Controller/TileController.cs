using Godot;
using LibreAutoTile.GUI.Editor.Model;

namespace LibreAutoTile.GUI.Editor.Controller;

public class TileController
{
  private readonly EditorModel model;
  private readonly GuiLogger logger;
  private readonly EditorRedrawer redrawer;

  public event Action<int>? TileDeleted;
  public event Action<int, int>? TileIdChanged;

  public TileController(EditorContext context)
  {
    model = context.Model;
    logger = context.Logger;
    redrawer = context.Redrawer;

    var tilesPanel = context.View.ToolsPanel.TilesPanel;
    tilesPanel.AddTileRequested += OnAddTileRequested;
    tilesPanel.RemoveRequested += OnTileRemoveRequested;
    tilesPanel.NameSubmitted += OnTileNameSubmitted;
    tilesPanel.IdSubmitted += OnTileIdSubmitted;
    tilesPanel.ConnectionGroupSubmitted += OnTileConnectionGroupSubmitted;
    tilesPanel.SelectRequested += OnTileSelectRequested;
    tilesPanel.MoveUpRequested += OnTileMoveUpRequested;
    tilesPanel.MoveDownRequested += OnTileMoveDownRequested;
    tilesPanel.ColorChanged += OnTileColorChanged;
    tilesPanel.WildcardIdChanged += OnWildcardIdChanged;
  }

  private void OnAddTileRequested()
  {
    var tile = model.Tiles.AddNew();
    redrawer.RefreshTilesView();
    logger.Log($"Added new tile: {tile.TileName}");
  }

  private void OnTileRemoveRequested(TileModel tile)
  {
    TileDeleted?.Invoke(tile.TileId);
    model.Tiles.Remove(tile);
    redrawer.RefreshTilesView();
    logger.Log($"Removed tile {tile.TileName}");
  }

  private void OnTileNameSubmitted(TileModel tile, string name)
  {
    model.Tiles.TryChangeName(tile, name);
    redrawer.RefreshTilesView();
  }

  private void OnTileIdSubmitted(TileModel tile, string text)
  {
    int oldId = tile.TileId;
    model.Tiles.TryChangeId(tile, text);
    if (tile.TileId != oldId)
      TileIdChanged?.Invoke(tile.TileId, oldId);
    redrawer.RefreshTilesView();
  }

  private void OnTileConnectionGroupSubmitted(TileModel tile, string text)
  {
    model.Tiles.TryChangeConnectionGroup(tile, text);
    redrawer.RefreshTilesView();
  }

  private void OnTileSelectRequested(TileModel tile)
  {
    model.Tiles.SetActive(tile);
    redrawer.RefreshTilesView();
    redrawer.RedrawBitmask();
  }

  private void OnTileMoveUpRequested(TileModel tile)
  {
    model.Tiles.MoveUp(tile);
    redrawer.RefreshTilesView();
  }

  private void OnTileMoveDownRequested(TileModel tile)
  {
    model.Tiles.MoveDown(tile);
    redrawer.RefreshTilesView();
  }

  private void OnTileColorChanged(TileModel tile, Color color)
  {
    tile.Color = color;
    redrawer.RedrawBitmask();
  }

  private void OnWildcardIdChanged(string text)
  {
    if (string.IsNullOrWhiteSpace(text))
    {
      model.WildcardId = null;
      return;
    }

    if (int.TryParse(text, out int wildcardId) && wildcardId >= 0)
      model.WildcardId = wildcardId;
  }
}
