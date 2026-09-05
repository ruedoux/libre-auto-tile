## New Features

- **Isometric tile support** — new `TileShape` enum (`Square`/`Isometric`); the Godot bindings now render isometric tilemaps. Includes an `Isometric.json` sample configuration and tileset
- **Tile-atlas probability** — tile definitions can assign a `Chance` weight to each mask/atlas entry, enabling probabilistic tile selection
- **GUI rewritten to MVC**

## Performance

- ~50x faster than Godot's built-in terrain implementation (relative, single-threaded comparison)
- Added tile-mask caching (`TileMaskCache`) and chunking improvement.
- Concurrent/parallel index searching and multi-layer tile placement
- Expanded BenchmarkDotNet suite (autotiler, concurrent, tile-mask searcher)

## Fixes

- Project/`.csproj` cleanup and shared `Directory.Build.props`
- Expanded tests for caching, atlas resolution, searchers, and tile-mask data
