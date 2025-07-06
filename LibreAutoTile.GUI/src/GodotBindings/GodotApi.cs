using System;
using Godot;

namespace Qwaitumin.LibreAutoTile.GUI.GodotBindings;

public static class GodotApi
{
  public static bool IsMouseOnElement(Control control)
    => control.GetGlobalRect().HasPoint(control.GetGlobalMousePosition());

  public static bool IsMouseOnElements(Control[] controls)
  {
    foreach (var control in controls)
      if (IsMouseOnElement(control)) return true;
    return false;
  }

  public static void FillOptionButtonWithEnum<T>(
      OptionButton enumOptionButton, T defaultValue) where T : struct, Enum
  {
    enumOptionButton.Clear();
    foreach (var enumValue in Enum.GetValues<T>())
      enumOptionButton.AddItem(enumValue.ToString());
    enumOptionButton.Select(Convert.ToInt32(defaultValue));
  }

  public static T AddChild<T>(Node parent, T child) where T : Node
  {
    parent.AddChild(child);
    return child;
  }
}