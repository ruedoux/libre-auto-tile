# AutoTile

Implementation of autotile algorithm for tilemaps with json configuration.

## Features

- **Autotiling pipeline**
- **Fully async compatible**
- **Tile configuration** (includes bitmasks and connections)

## Tests

Wrote tests for most important parts of the library to ensure they work as expected. To run tests just `cd tests` from project root, build and run.

## Installation

### Manual reference to .csproj for git project

Add the submodule outside of your .csproj file, example:

```bash
# Add submodule to your project
git submodule add https://github.com/ruedoux/autotile
git submodule update --init --recursive

# Go to your main project path and reference the submodule
cd /your/project/path
dotnet add reference ../path/to/gamecore/lib/AutoTile.csproj
```

### Manual reference to dll

```bash
# Create release .dll
git clone https://github.com/ruedoux/autotile
cd gamecore/lib
dotnet build -c Release

# Add dll to the project
cd /path/to/the/project
dotnet add package /path/to/compiled/AutoTile.dll
```
