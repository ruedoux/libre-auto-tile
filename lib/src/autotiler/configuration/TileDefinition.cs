using System.Numerics;

namespace Qwaitumin.AutoTile;

public record TileDefinition(
    uint Layer = 0,
    string ImageFileName = "<NONE>",
    Vector2 PositionInSet = default,
    string BitmaskName = "<NONE>",
    int AutoTileGroup = 0);