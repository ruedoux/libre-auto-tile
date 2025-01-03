using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace Qwaitumin.AutoTile.GUI.Core.GodotBindings;

internal class ActionWithCooldown(Action action, long cooldownMs)
{
  private readonly Stopwatch stopwatch = Stopwatch.StartNew();

  public bool TryAction()
  {
    if (!IsReady())
      return false;

    action();
    stopwatch.Restart();
    return true;
  }

  private bool IsReady()
    => stopwatch.ElapsedMilliseconds >= cooldownMs;
}

public class GodotInputListener
{
  public bool Active = true;

  private readonly Dictionary<string, ActionWithCooldown> inputKeyActions = [];
  private readonly Dictionary<string, ActionWithCooldown> processKeyActions = [];
  private readonly HashSet<Action<InputEventMouseMotion>> inputMouseMotionActions = [];
  private readonly HashSet<Action<InputEventMouseButton>> inputMouseButtonActions = [];
  private readonly HashSet<Action> processActions = [];

  public void AddInputAction(string actionName, long cooldownMs, Action action)
  {
    if (!InputMap.GetActions().Contains(actionName))
      throw new ArgumentException($"InputMap does not contain action with name {actionName}! This action should be added to KeyMap.json");

    inputKeyActions[actionName] = new(action, cooldownMs);
  }

  public void AddProcessActionByName(string actionName, long cooldownMs, Action action)
  {
    if (!InputMap.GetActions().Contains(actionName))
      throw new ArgumentException($"InputMap does not contain action with name {actionName}! This action should be added to KeyMap.json");

    processKeyActions[actionName] = new(action, cooldownMs);
  }

  public void AddInputMouseMotionAction(Action<InputEventMouseMotion> action)
    => inputMouseMotionActions.Add(action);

  public void AddInputMouseButtonAction(Action<InputEventMouseButton> action)
    => inputMouseButtonActions.Add(action);

  public void AddProcessAction(Action action)
    => processActions.Add(action);

  public void ListenToInput(InputEvent inputEvent)
  {
    if (!Active)
      return;

    foreach (string actionName in inputKeyActions.Keys)
      if (inputEvent.IsActionPressed(actionName))
        inputKeyActions[actionName].TryAction();

    if (inputEvent is InputEventMouseButton mouseButton)
      foreach (var inputMouseButtonAction in inputMouseButtonActions)
        inputMouseButtonAction(mouseButton);

    if (inputEvent is InputEventMouseMotion mouseMotion)
      foreach (var inputMouseMotionAction in inputMouseMotionActions)
        inputMouseMotionAction(mouseMotion);
  }

  public void ListenToProcess()
  {
    if (!Active)
      return;

    foreach (string actionName in processKeyActions.Keys)
      if (Input.IsActionPressed(actionName))
        processKeyActions[actionName].TryAction();

    foreach (var processAction in processActions)
      processAction();
  }
}