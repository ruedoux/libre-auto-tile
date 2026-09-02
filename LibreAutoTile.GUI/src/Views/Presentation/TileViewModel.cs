using Godot;

namespace Qwaitumin.LibreAutoTile.GUI.Views.Presentation;

public sealed record TileViewModel(
  object Key,
  int TileId,
  string TileName,
  Color Color,
  uint? ConnectionGroup,
  bool IsActive);
