using Qwaitumin.LibreAutoTile.GUI.Models;
using Qwaitumin.LibreAutoTile.GUI.Views.Presentation;

namespace Qwaitumin.LibreAutoTile.GUI.Controllers;

public static class TileViewMapper
{
  public static TileViewModel ToViewModel(TileModel tile, bool isActive)
    => new(tile, tile.TileId, tile.TileName, tile.Color, tile.ConnectionGroup, isActive);

  public static IReadOnlyList<TileViewModel> ToViewModels(TileCollection tiles)
    => [.. tiles.Tiles.Select(tile => ToViewModel(tile, tiles.ActiveTile == tile))];

  public static TileModel ToModel(TileViewModel viewModel)
    => (TileModel)viewModel.Key;
}
