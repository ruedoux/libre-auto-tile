using System.Numerics;

namespace Qwaitumin.AutoTile;

public record TileIdentificator(int TileId, string TileName);
public record TileResource(
  string ImagePath,
  uint Layer,
  Dictionary<byte, Vector2> BitmaskSet,
  int AutoTileGroup);


public class TileLoader
{
  private readonly string imageDirectoryPath;
  private readonly AutoTileConfig autoTileConfig;
  private readonly string[] tileIdToTileNames;

  public TileLoader(
    string imageDirectoryPath,
    AutoTileConfig autoTileConfig,
    string[] tileNameToIds)
  {
    this.imageDirectoryPath = imageDirectoryPath;
    this.autoTileConfig = autoTileConfig;
    this.tileIdToTileNames = tileNameToIds;
  }

  public Dictionary<TileIdentificator, TileResource> LoadTiles()
  {
    if (autoTileConfig.TileDefinitions.Count != tileIdToTileNames.Length)
      throw new ArgumentException($"TileSetConfig defined tiles count should be equal to passed tile names array: {autoTileConfig.TileDefinitions.Count} != {tileIdToTileNames.Length}");

    foreach (var (tileName, _) in autoTileConfig.TileDefinitions)
      if (Array.FindIndex(tileIdToTileNames, name => name == tileName) < 0)
        throw new ArgumentException($"TileSetConfig contains tile '{tileName}', but passes tile names array does not contain it");

    if (!Directory.Exists(imageDirectoryPath))
      throw new DirectoryNotFoundException($"Directory does not exist: {imageDirectoryPath}");

    Dictionary<string, string> fileNamesToPaths = GetFileNamesToPaths(imageDirectoryPath);
    Dictionary<TileIdentificator, TileResource> tiles = GetTileIdentificators()
      .ToDictionary(
          tileIdentificator => tileIdentificator,
          tileIdentificator => GetTileResource(tileIdentificator, fileNamesToPaths));

    return tiles;
  }

  private TileIdentificator[] GetTileIdentificators()
    => Enumerable.Range(0, tileIdToTileNames.Length)
    .Select(tileId => new TileIdentificator(tileId, tileIdToTileNames[tileId]))
    .ToArray();

  private TileResource GetTileResource(
    TileIdentificator tileIdentificator, Dictionary<string, string> fileNamesToPaths)
  {
    var tileDefinition = autoTileConfig.TileDefinitions[tileIdentificator.TileName];
    if (!fileNamesToPaths.TryGetValue(tileDefinition.ImageFileName, out string? imagePath))
      throw new ArgumentException($"Missing required image: '{tileDefinition.ImageFileName}' in path: '{imageDirectoryPath}'");

    var defaultBitmaskSet = autoTileConfig.BitmaskSets[tileDefinition.BitmaskName];
    var shiftedDefaultBitmaskSet = ShiftBitmask(new(defaultBitmaskSet), tileDefinition.PositionInSet);

    return new(
      ImagePath: imagePath,
      Layer: tileDefinition.Layer,
      BitmaskSet: shiftedDefaultBitmaskSet,
      AutoTileGroup: tileDefinition.AutoTileGroup
    );
  }

  private static Dictionary<string, string> GetFileNamesToPaths(string imageDirectoryPath)
    => Directory.GetFiles(imageDirectoryPath, $"*", SearchOption.AllDirectories)
        .ToDictionary(p => Path.GetFileName(p), p => p);

  private static Dictionary<byte, Vector2> ShiftBitmask(Dictionary<byte, Vector2> bitmaskSet, Vector2 positionInSet)
    => bitmaskSet
        .Select(kv => new KeyValuePair<byte, Vector2>(kv.Key, kv.Value + positionInSet))
        .ToDictionary();
}