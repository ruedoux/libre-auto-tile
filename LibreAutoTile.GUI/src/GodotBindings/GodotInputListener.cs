using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace Qwaitumin.LibreAutoTile.GUI.GodotBindings;

internal class ListenerAction<T>(
  Action<T> action,
  Func<T, bool>? condition = null,
  long cooldownMs = 0)
{
  private readonly Stopwatch stopwatch = Stopwatch.StartNew();

  public bool TryAction(T value)
  {
    if (stopwatch.ElapsedMilliseconds < cooldownMs)
      return false;

    if (condition is not null)
      if (!condition(value))
        return false;

    action(value);
    stopwatch.Restart();
    return true;
  }
}

public class GodotInputListener
{
  public bool Active = true;

  private readonly HashSet<ListenerAction<InputEvent>> inputActions = [];
  private readonly HashSet<ListenerAction<InputEventMouse>> inputMouseActions = [];
  private readonly HashSet<ListenerAction<InputEventMouseButton>> inputMouseButtonActions = [];
  private readonly HashSet<ListenerAction<InputEventMouseMotion>> inputMouseMotionActions = [];
  private readonly HashSet<ListenerAction<double>> processActions = [];

  public void AddInputAction(Action<InputEvent> action, Func<InputEvent, bool>? condition = null, long cooldownMs = 0)
    => inputActions.Add(new(action, condition, cooldownMs));

  public void AddInputMouseAction(Action<InputEventMouse> action, Func<InputEventMouse, bool>? condition = null, long cooldownMs = 0)
    => inputMouseActions.Add(new(action, condition, cooldownMs));

  public void AddInputMouseButtonAction(Action<InputEventMouseButton> action, Func<InputEventMouseButton, bool>? condition = null, long cooldownMs = 0)
    => inputMouseButtonActions.Add(new(action, condition, cooldownMs));

  public void AddInputMouseMotionAction(Action<InputEventMouseMotion> action, Func<InputEventMouseMotion, bool>? condition = null, long cooldownMs = 0)
    => inputMouseMotionActions.Add(new(action, condition, cooldownMs));

  public void AddProcessAction(Action<double> action, Func<double, bool>? condition = null, long cooldownMs = 0)
    => processActions.Add(new(action, condition, cooldownMs));

  public void ListenToInput(InputEvent inputEvent)
  {
    if (!Active)
      return;

    foreach (var action in inputActions)
      action.TryAction(inputEvent);

    foreach (var action in inputMouseActions)
      if (inputEvent is InputEventMouse inputEventType)
        action.TryAction(inputEventType);

    foreach (var action in inputMouseButtonActions)
      if (inputEvent is InputEventMouseButton inputEventType)
        action.TryAction(inputEventType);

    foreach (var action in inputMouseMotionActions)
      if (inputEvent is InputEventMouseMotion inputEventType)
        action.TryAction(inputEventType);
  }

  public void ListenToProcess(double delta)
  {
    if (!Active)
      return;

    foreach (var action in processActions)
      action.TryAction(delta);
  }
}