using Godot;
using Qwaitumin.AutoTile.GUI.Core;

namespace Qwaitumin.AutoTile.GUI.Scenes.Editor.Preview;


public partial class EditorPreview : MarginContainer, IState
{
  public void InitializeState()
    => Show();

  public void EndState()
    => Hide();
}