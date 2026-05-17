using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.Configuration.Models;
using Qwaitumin.LibreAutoTile.Tiling;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.LibreAutoTile.Tests.Tiling;


[TestClass]
public class TilingSetConnectionGroupTest
{
  private string jsonString = "";

  [BeforeAll]
  public void BeforeAll()
  {
    jsonString = File.ReadAllText("../resources/configurations/ExampleConfigurationConnectionGroups.json");
  }

  [TestMethod]
  public void PlaceTile_CorrectlyPlacesSingleTileFilledSquare_WhenMapEmpty()
  {
    // Given
    var autoTileConfiguration = AutoTileConfiguration.FromJsonString(jsonString)
      ?? throw new ArgumentException();
    AutoTiler autoTiler = new(1, AutoTileConfigurationExtractor.BuildTileIdToTileMaskSearcher(autoTileConfiguration));
    TilingStateVerifier tilingStateVerifier = new(autoTiler, autoTileConfiguration);

    // When
    // Then
    tilingStateVerifier.AddTile(0, Vector2.Zero, new(-1, -1, -1, -1, -1, -1, -1, -1));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(0, Vector2.Top, new(-1, -1, -1, -1, -1, 0, -1, -1));
    tilingStateVerifier.UpdateTile(0, Vector2.Zero, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(0, Vector2.Bottom, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tilingStateVerifier.UpdateTile(0, Vector2.Zero, new(-1, 0, -1, -1, -1, 0, -1, -1));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(0, Vector2.Left, new(-1, -1, -1, 0, -1, -1, -1, -1));
    tilingStateVerifier.UpdateTile(0, Vector2.Zero, new(-1, 0, -1, -1, -1, 0, -1, 0));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(0, Vector2.Right, new(-1, -1, -1, -1, -1, -1, -1, 0));
    tilingStateVerifier.UpdateTile(0, Vector2.Zero, new(-1, 0, -1, 0, -1, 0, -1, 0));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(0, Vector2.TopLeft, new(-1, -1, -1, 0, 0, 0, -1, -1));
    tilingStateVerifier.UpdateTile(0, Vector2.Zero, new(0, 0, -1, 0, -1, 0, -1, 0));
    tilingStateVerifier.UpdateTile(0, Vector2.Top, new(-1, -1, -1, -1, -1, 0, 0, 0));
    tilingStateVerifier.UpdateTile(0, Vector2.Left, new(-1, 0, 0, 0, -1, -1, -1, -1));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(0, Vector2.TopRight, new(-1, -1, -1, -1, -1, 0, 0, 0));
    tilingStateVerifier.UpdateTile(0, Vector2.Zero, new(0, 0, 0, 0, -1, 0, -1, 0));
    tilingStateVerifier.UpdateTile(0, Vector2.Top, new(-1, -1, -1, 0, 0, 0, 0, 0));
    tilingStateVerifier.UpdateTile(0, Vector2.Right, new(0, 0, -1, -1, -1, -1, -1, 0));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(0, Vector2.BottomRight, new(0, 0, -1, -1, -1, -1, -1, 0));
    tilingStateVerifier.UpdateTile(0, Vector2.Zero, new(0, 0, 0, 0, 0, 0, -1, 0));
    tilingStateVerifier.UpdateTile(0, Vector2.Bottom, new(-1, 0, 0, 0, -1, -1, -1, -1));
    tilingStateVerifier.UpdateTile(0, Vector2.Right, new(0, 0, -1, -1, -1, 0, 0, 0));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(0, Vector2.BottomLeft, new(-1, 0, 0, 0, -1, -1, -1, -1));
    tilingStateVerifier.UpdateTile(0, Vector2.Zero, new(0, 0, 0, 0, 0, 0, 0, 0));
    tilingStateVerifier.UpdateTile(0, Vector2.Bottom, new(0, 0, 0, 0, -1, -1, -1, 0));
    tilingStateVerifier.UpdateTile(0, Vector2.Left, new(-1, 0, 0, 0, 0, 0, -1, -1));
    tilingStateVerifier.Verify();
  }

  [TestMethod]
  public void PlaceTile_CorrectlyPlacesMultipleTileFilledSquare_WhenTilesAlternated()
  {
    // Given
    var autoTileConfiguration = AutoTileConfiguration.FromJsonString(jsonString)
      ?? throw new ArgumentException();
    AutoTiler autoTiler = new(1, AutoTileConfigurationExtractor.BuildTileIdToTileMaskSearcher(autoTileConfiguration));
    TilingStateVerifier tilingStateVerifier = new(autoTiler, autoTileConfiguration);

    // When
    // Then
    tilingStateVerifier.AddTile(0, Vector2.Zero, new(-1, -1, -1, -1, -1, -1, -1, -1));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(1, Vector2.Top, new(-1, -1, -1, -1, -1, 1, -1, -1));
    tilingStateVerifier.UpdateTile(0, Vector2.Zero, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(0, Vector2.Bottom, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tilingStateVerifier.UpdateTile(0, Vector2.Zero, new(-1, 0, -1, -1, -1, 0, -1, -1));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(1, Vector2.Left, new(-1, -1, -1, 1, -1, -1, -1, -1));
    tilingStateVerifier.UpdateTile(0, Vector2.Zero, new(-1, 0, -1, -1, -1, 0, -1, 0));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(0, Vector2.Right, new(-1, -1, -1, -1, -1, -1, -1, 0));
    tilingStateVerifier.UpdateTile(0, Vector2.Zero, new(-1, 0, -1, 0, -1, 0, -1, 0));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(1, Vector2.TopLeft, new(-1, -1, -1, 1, 1, 1, -1, -1));
    tilingStateVerifier.UpdateTile(0, Vector2.Zero, new(0, 0, -1, 0, -1, 0, -1, 0));
    tilingStateVerifier.UpdateTile(1, Vector2.Top, new(-1, -1, -1, -1, -1, 1, 1, 1));
    tilingStateVerifier.UpdateTile(1, Vector2.Left, new(-1, 1, 1, 1, -1, -1, -1, -1));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(0, Vector2.TopRight, new(-1, -1, -1, -1, -1, 0, 0, 0));
    tilingStateVerifier.UpdateTile(0, Vector2.Zero, new(0, 0, 0, 0, -1, 0, -1, 0));
    tilingStateVerifier.UpdateTile(1, Vector2.Top, new(-1, -1, -1, 1, 1, 1, 1, 1));
    tilingStateVerifier.UpdateTile(0, Vector2.Right, new(0, 0, -1, -1, -1, -1, -1, 0));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(1, Vector2.BottomRight, new(1, 1, -1, -1, -1, -1, -1, 1));
    tilingStateVerifier.UpdateTile(0, Vector2.Zero, new(0, 0, 0, 0, 0, 0, -1, 0));
    tilingStateVerifier.UpdateTile(0, Vector2.Bottom, new(-1, 0, 0, 0, -1, -1, -1, -1));
    tilingStateVerifier.UpdateTile(0, Vector2.Right, new(0, 0, -1, -1, -1, 0, 0, 0));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(0, Vector2.BottomLeft, new(-1, 0, 0, 0, -1, -1, -1, -1));
    tilingStateVerifier.UpdateTile(0, Vector2.Zero, new(0, 0, 0, 0, 0, 0, 0, 0));
    tilingStateVerifier.UpdateTile(0, Vector2.Bottom, new(0, 0, 0, 0, -1, -1, -1, 0));
    tilingStateVerifier.UpdateTile(1, Vector2.Left, new(-1, 1, 1, 1, 1, 1, -1, -1));
    tilingStateVerifier.Verify();
  }

  [TestMethod]
  public void PlaceTile_CorrectlyPlacesMultipleTileFilledSquare_WhenTilesRefilled()
  {
    // Given
    var autoTileConfiguration = AutoTileConfiguration.FromJsonString(jsonString)
      ?? throw new ArgumentException();
    AutoTiler autoTiler = new(1, AutoTileConfigurationExtractor.BuildTileIdToTileMaskSearcher(autoTileConfiguration));
    TilingStateVerifier tilingStateVerifier = new(autoTiler, autoTileConfiguration);

    // When
    // Then
    tilingStateVerifier.AddTile(0, Vector2.Zero, new(0, 0, 0, 0, 0, 0, 0, 0));
    tilingStateVerifier.AddTile(0, Vector2.Top, new(-1, -1, -1, 0, 0, 0, 0, 0));
    tilingStateVerifier.AddTile(0, Vector2.Bottom, new(0, 0, 0, 0, -1, -1, -1, 0));
    tilingStateVerifier.AddTile(0, Vector2.Left, new(-1, 0, 0, 0, 0, 0, -1, -1));
    tilingStateVerifier.AddTile(0, Vector2.Right, new(0, 0, -1, -1, -1, 0, 0, 0));
    tilingStateVerifier.AddTile(0, Vector2.TopLeft, new(-1, -1, -1, 0, 0, 0, -1, -1));
    tilingStateVerifier.AddTile(0, Vector2.TopRight, new(-1, -1, -1, -1, -1, 0, 0, 0));
    tilingStateVerifier.AddTile(0, Vector2.BottomRight, new(0, 0, -1, -1, -1, -1, -1, 0));
    tilingStateVerifier.AddTile(0, Vector2.BottomLeft, new(-1, 0, 0, 0, -1, -1, -1, -1));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(1, Vector2.Zero, new(1, 1, 1, 1, 1, 1, 1, 1));
    tilingStateVerifier.Verify();
    tilingStateVerifier.AddTile(1, Vector2.Top, new(-1, -1, -1, 1, 1, 1, 1, 1));
    tilingStateVerifier.Verify();
    tilingStateVerifier.AddTile(1, Vector2.Bottom, new(1, 1, 1, 1, -1, -1, -1, 1));
    tilingStateVerifier.Verify();
    tilingStateVerifier.AddTile(1, Vector2.Left, new(-1, 1, 1, 1, 1, 1, -1, -1));
    tilingStateVerifier.Verify();
    tilingStateVerifier.AddTile(1, Vector2.Right, new(1, 1, -1, -1, -1, 1, 1, 1));
    tilingStateVerifier.Verify();
    tilingStateVerifier.AddTile(1, Vector2.TopLeft, new(-1, -1, -1, 1, 1, 1, -1, -1));
    tilingStateVerifier.Verify();
    tilingStateVerifier.AddTile(1, Vector2.TopRight, new(-1, -1, -1, -1, -1, 1, 1, 1));
    tilingStateVerifier.Verify();
    tilingStateVerifier.AddTile(1, Vector2.BottomRight, new(1, 1, -1, -1, -1, -1, -1, 1));
    tilingStateVerifier.Verify();
    tilingStateVerifier.AddTile(1, Vector2.BottomLeft, new(-1, 1, 1, 1, -1, -1, -1, -1));
    tilingStateVerifier.Verify();
  }

}
