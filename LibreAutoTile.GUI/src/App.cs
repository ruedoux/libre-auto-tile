using LibreAutoTile.GUI.Controllers;
using LibreAutoTile.GUI.Models;
using LibreAutoTile.GUI.Views;

namespace LibreAutoTile.GUI;

public static class App
{
  public static void Run(EditorScene view)
  {
    var data = new EditorData();
    _ = new EditorController(data, view);
  }
}
