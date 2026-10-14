using LibreAutoTile.Configuration;
using LibreAutoTile.Configuration.Models;
using LibreAutoTile.Tiling;
using SimpleTest;

namespace LibreAutoTile.Tests.Tiling;

[TestClass]
public class AutoTileConfigurationExtractorTest
{
  [TestMethod]
  public void GetImageFileNameToAtlasPositions_CollectsAllAtlasPositions_WhenMultipleDefinitions()
  {
    // Given
    TileMaskDefinition tileMaskDefinition = TileMaskDefinition.Construct(new()
    {
      { new Vector3(1, 2, 0), [TileMaskData.Construct([0, 0, 0, 0, 0, 0, 0, 0], 1)] },
      { new Vector3(3, 4, 1), [TileMaskData.Construct([0, 0, 0, 0, 0, 0, 0, 0], 1)] },
    });
    AutoTileConfiguration config = AutoTileConfiguration.Construct(32, new()
    {
      { 0, TileDefinition.Construct(new() { { "a.png", tileMaskDefinition } }) },
    });

    // When
    var result = AutoTileConfigurationExtractor.GetImageFileNameToAtlasPositions(config);

    // Then
    Assertions.AssertTrue(result.TryGetValue("a.png", out var positions));
    Assertions.AssertEqual(2, positions!.Count);
    Assertions.AssertTrue(positions.Contains(new Vector2(1, 2)));
    Assertions.AssertTrue(positions.Contains(new Vector2(3, 4)));
  }

  [TestMethod]
  public void GetImageFileNameToAtlasPositions_DeduplicatesPositions_WhenRepeated()
  {
    // Given
    TileMaskDefinition tileMaskDefinition = TileMaskDefinition.Construct(new()
    {
      { new Vector3(1, 2, 0), [TileMaskData.Construct([0, 0, 0, 0, 0, 0, 0, 0], 1)] },
      { new Vector3(1, 2, 1), [TileMaskData.Construct([0, 0, 0, 0, 0, 0, 0, 0], 1)] },
    });
    AutoTileConfiguration config = AutoTileConfiguration.Construct(32, new()
    {
      { 0, TileDefinition.Construct(new() { { "a.png", tileMaskDefinition } }) },
    });

    // When
    var result = AutoTileConfigurationExtractor.GetImageFileNameToAtlasPositions(config);

    // Then
    Assertions.AssertTrue(result.TryGetValue("a.png", out var positions));
    Assertions.AssertEqual(1, positions!.Count);
  }
}
