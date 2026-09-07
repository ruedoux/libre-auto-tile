using Godot;

namespace Qwaitumin.LibreAutoTile.GUI.Views;

public partial class LayerControl : HBoxContainer
{
  public readonly SpinBox LayerSpinBox;

  public LayerControl()
  {
    this.AppendLabel("Layer").ExpandVertical().FitContent().DisableAutowrap();

    LayerSpinBox = this.AppendSpinBox();
    LayerSpinBox.MinValue = 0;
    LayerSpinBox.MaxValue = int.MaxValue;
    LayerSpinBox.Step = 1;
    LayerSpinBox.GetLineEdit().AddThemeConstantOverride("minimum_character_width", 2);

    SetLayer(0);
  }

  public void SetLayer(int layer)
    => LayerSpinBox.SetValueNoSignal(layer);
}
