# LibreAutoTile GUI

A dedicated Godot-based GUI for visually creating LibreAutoTile JSON configurations.

## Building

Export prebuilt binaries with the repository build script (requires Godot and [rcedit](https://github.com/electron/rcedit)):

```sh
./build.sh --build-gui
```

This produces `build/export/linux-gui.tar.gz` and `build/export/windows-gui.tar.gz`.

## Structure

The GUI is written in an MVC style:

- `src/Models/` — application data and configuration conversion.
- `src/Views/` — Godot scene nodes and drawing.
- `src/Controllers/` — editors for tiles, bitmasks, probability, settings, and preview.
- `src/App.cs` — application entry point wiring model and view together.
