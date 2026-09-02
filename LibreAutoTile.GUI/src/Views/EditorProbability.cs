using Godot;

namespace Qwaitumin.LibreAutoTile.GUI.Views;

public partial class EditorProbability : MarginContainer
{
  public readonly SpinBox ProbabilitySpinBox;
  private readonly RichTextLabel positionLabel;

  public EditorProbability()
  {
    this.ExpandFill();

    var vbox = this.AppendVBox().ExpandFill();

    var positionRow = vbox.AppendHBox().ExpandHorizontal();
    positionLabel = positionRow.AppendLabel("Position (0,0)")
      .ExpandHorizontal()
      .ExpandVertical()
      .FitContent()
      .DisableAutowrap();

    ProbabilitySpinBox = positionRow.AppendSpinBox().ExpandHorizontal();
    ProbabilitySpinBox.MinValue = 0;
    ProbabilitySpinBox.MaxValue = uint.MaxValue;
    ProbabilitySpinBox.Step = 1;
    SetProbabilityEnabled(false);
  }

  public void SetSelectedPosition(Vector2I? position)
  {
    var p = position ?? Vector2I.Zero;
    positionLabel.Text = $"Position ({p.X},{p.Y})";
  }

  public void SetProbabilityValue(uint value)
    => ProbabilitySpinBox.SetValueNoSignal(value);

  public void SetProbabilityEnabled(bool enabled)
  {
    ProbabilitySpinBox.Editable = enabled;
    ProbabilitySpinBox.MouseFilter = enabled
      ? MouseFilterEnum.Stop
      : MouseFilterEnum.Ignore;
  }
}
