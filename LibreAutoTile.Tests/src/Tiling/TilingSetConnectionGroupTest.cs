using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.Configuration.Models;
using Qwaitumin.LibreAutoTile.Tiling;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.LibreAutoTile.Tests.Tiling;


[SimpleTestClass]
public class TilingSetConnectionGroupTest
{
  private string jsonString = "";

  [SimpleBeforeAll]
  public void BeforeAll()
  {
    jsonString = File.ReadAllText("../resources/configurations/ExampleConfigurationConnectionGroups.json");
  }

}
