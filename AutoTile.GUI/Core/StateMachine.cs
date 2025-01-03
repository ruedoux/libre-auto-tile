using System;
using System.Collections.Frozen;
using System.Collections.Generic;

namespace Qwaitumin.AutoTile.GUI.Core;

public interface IState
{
  public void InitializeState();
  public void EndState();
}

public class StateMachine<StateEnum>
  where StateEnum : struct, Enum
{
  public IState CurrentState { private set; get; }
  public readonly FrozenDictionary<StateEnum, IState> EnumValueToState;

  public StateMachine(StateEnum initialEnumValue, Dictionary<StateEnum, IState> states)
  {
    var enumValues = Enum.GetValues<StateEnum>();
    foreach (StateEnum enumValue in enumValues)
      if (!states.ContainsKey(enumValue))
        throw new ArgumentException($"State is not present in the state dictionary: '{enumValue}'. All enum values must be present");

    EnumValueToState = states.ToFrozenDictionary();
    CurrentState = states[initialEnumValue];
    CurrentState.InitializeState();
  }

  public void SwitchStateTo(StateEnum enumValue)
  {
    var newState = EnumValueToState[enumValue];
    if (CurrentState == newState)
      return;

    CurrentState.EndState();
    newState.InitializeState();
    CurrentState = newState;
  }
}