# LibreAutoTile Godot Example

A Godot project demonstrating how to use the Godot bindings for LibreAutoTile.

## Scenes

- `Scenes/Examples/SimpleExample` — randomly fills a 32x32 grid and draws it with `AutoTileMap`.
- `Scenes/Examples/ProceduralExample` — generates a 64x64 map from simplex noise and draws it.
- `Scenes/Comparasion/Compare` — lets you pick a map size and choose between LibreAutoTile and Godot's
  built-in terrain autotiling, then measures how long the map takes to render.

The examples load their configuration from `../resources/configurations/`.
