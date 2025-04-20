using System.Collections.Generic;
using Godot;
using Qwaitumin.AutoTile.GUI.Core;
using Qwaitumin.AutoTile.GUI.Core.Signals;
using Qwaitumin.AutoTile.GUI.Scenes.Editor.Tiles;

namespace Qwaitumin.AutoTile.GUI.Scenes.Editor.Preview;


public partial class EditorPreview : MarginContainer, IState
{
  private readonly EventNotifier<bool> EnteredPreview = new();
  private readonly EventNotifier<bool> ExitedPreview = new();

  public void AddCreatedTiles(HashSet<GuiTile> CreatedTiles)
  {

  }

  public void InitializeState()
  {
    Show();
    EnteredPreview.NotifyObservers(true);
  }

  public void EndState()
  {
    Hide();
    ExitedPreview.NotifyObservers(true);
  }
}