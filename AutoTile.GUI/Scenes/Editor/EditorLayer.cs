using Godot;
using Qwaitumin.AutoTile.GUI.Core.Signals;

namespace Qwaitumin.AutoTile.GUI.Scenes.Editor;

public partial class EditorLayer : HBoxContainer
{
  public Button AddButton { private set; get; } = null!;
  public Button SubButton { private set; get; } = null!;
  public RichTextLabel RichTextLabel { private set; get; } = null!;

  public readonly ObservableVariable<uint> LayerObservable = new(0);

  public override void _Ready()
  {
    AddButton = GetNode<Button>("Add");
    SubButton = GetNode<Button>("Sub");
    RichTextLabel = GetNode<RichTextLabel>("P/RichTextLabel");

    AddButton.Pressed += AddLayer;
    SubButton.Pressed += SubLayer;
    UpdateText();
  }

  private void AddLayer()
  {
    LayerObservable.ChangeValueAndNotifyObservers(LayerObservable.Value + 1);
    UpdateText();
  }

  private void SubLayer()
  {
    if (LayerObservable.Value != 0)
      LayerObservable.ChangeValueAndNotifyObservers(LayerObservable.Value - 1);
    else
      LayerObservable.ChangeValueAndNotifyObservers(0);
    UpdateText();
  }

  private void UpdateText()
  {
    RichTextLabel.Text = $" Layer {LayerObservable.Value} ";
  }
}
