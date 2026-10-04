using Qwaitumin.LibreAutoTile.Configuration.Models;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.LibreAutoTile.Tests.Configuration;

[TestClass]
public class TileShapeTest
{
  [TestMethod]
  public void GetCellSize_ReturnsSquareDimensions_WhenSquare()
    => Assertions.AssertEqual(new Vector2(32, 32), TileShape.Square.GetCellSize(32));

  [TestMethod]
  public void GetCellSize_ReturnsHalfHeight_WhenIsometric()
    => Assertions.AssertEqual(new Vector2(32, 16), TileShape.Isometric.GetCellSize(32));

  [TestMethod]
  public void GetCellSize_ClampsHeightToOne_WhenTileSizeSmall()
    => Assertions.AssertEqual(new Vector2(1, 1), TileShape.Isometric.GetCellSize(1));
}
