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

- `src/Editor/Model/` — application data and configuration conversion (`EditorModel`, `BitmaskDatabase`).
- `src/Editor/View/` — Godot scene nodes and drawing (including the `EditorView` component).
- `src/Editor/Controller/` — thin controllers for tiles, settings, configuration, camera, and input, wired by `EditorController` around an `EditorContext` bundle.
- `src/Editor/Controller/Tools/` — the editor tools, a state machine (`ToolStateMachine`) that maps each tab to an `IToolState` implementation (`TilesToolController`, `ProbabilityToolController`, `PreviewToolController`).
- `src/Settings/` — injected `AppSettings` (UI, editor, camera, and world sub-settings).
- `src/GuiLogger.cs` — injected logger.
- `Scenes/Editor/EditorScene.cs` — scene root wiring the model, view, controller, settings, and logger together.
