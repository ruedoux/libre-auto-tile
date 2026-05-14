using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.Configuration.Models;
using Qwaitumin.LibreAutoTile.Tiling;
using Qwaitumin.LibreAutoTile.Tiling.Search.Models;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.LibreAutoTile.Tests.Tiling;

public class TilingStateVerifier(AutoTiler autoTiler, AutoTileConfiguration autoTileConfiguration)
{
  private readonly AutoTiler autoTiler = autoTiler;
  private readonly AutoTileConfiguration autoTileConfiguration = autoTileConfiguration;
  private readonly Dictionary<Vector2, (int TileId, TileMask TileMask)> mappedExpectedTileMasks = [];

  public void UpdateTile(int tileId, Vector2 position, TileMask expectedTileMask)
  {
    mappedExpectedTileMasks[position] = new(tileId, expectedTileMask);
  }

  public void AddTile(int tileId, Vector2 position, TileMask expectedTileMask)
  {
    autoTiler.PlaceTile(0, position, tileId);
    mappedExpectedTileMasks[position] = new(tileId, expectedTileMask);
  }

  public void Verify()
  {
    foreach (var (position, maskPacked) in mappedExpectedTileMasks)
    {
      TileData resultTileData = autoTiler.GetTile(0, position);
      var expectedState = GetAtlasesAndMaskFromConfiguration(maskPacked.TileId, maskPacked.TileMask);
      bool isExpectedAtlas = expectedState.TileAtlases.Any(tileAtlas =>
        tileAtlas.Position == resultTileData.TileAtlas.Position
        && tileAtlas.ImageFileName == resultTileData.TileAtlas.ImageFileName);

      Assertions.AssertTrue(isExpectedAtlas,
        $"Mask at position {position} is {resultTileData.TileMask}, but should be: {expectedState.TileMask}, atlas should be one of: [{string.Join(", ", expectedState.TileAtlases)}] but is {resultTileData.TileAtlas}");
    }
  }

  private (TileAtlas[] TileAtlases, TileMask TileMask) GetAtlasesAndMaskFromConfiguration(
    int tileId, TileMask tileMask)
  {
    var tileDefinition = autoTileConfiguration.TileDefinitions[(uint)tileId];
    TileAtlas defaultTileAtlas = new(new(), "<None>");
    List<TileAtlas> matchingAtlases = [];

    foreach (var (imageFileName, tileMaskDefinition) in tileDefinition.ImageFileNameToTileMaskDefinition)
    {
      foreach (var (atlasPosition, tileMaskAndChanceArray) in tileMaskDefinition.AtlasPositionToTileMaskAndChance)
      {
        foreach (var (mask, chance) in tileMaskAndChanceArray)
        {
          var candidateTileMask = TileMask.FromArray([.. mask]);
          defaultTileAtlas = new(atlasPosition.ToVector2(), imageFileName, chance);

          if (candidateTileMask == tileMask)
            matchingAtlases.Add(defaultTileAtlas);
        }
      }
    }

    if (matchingAtlases.Count > 0)
      return ([.. matchingAtlases], tileMask);

    return ([defaultTileAtlas], new(-999));
  }
}
