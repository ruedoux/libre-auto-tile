using Qwaitumin.LibreAutoTile.GUI.Controllers;
using Qwaitumin.LibreAutoTile.GUI.Models;
using Qwaitumin.LibreAutoTile.GUI.Views;

namespace Qwaitumin.LibreAutoTile.GUI;

public static class App
{
  public static void Run(EditorScene view)
  {
    var data = new EditorData();
    _ = new EditorController(data, view);
  }
}
