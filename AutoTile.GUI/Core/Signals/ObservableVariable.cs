using System;
using System.Collections.Generic;

namespace Qwaitumin.AutoTile.GUI.Core.Signals;


public class ObservableVariable<T>
{
  private readonly List<Action<T>> observers = [];

  public T Value { private set; get; }

  public ObservableVariable(T initialValue, IEnumerable<Action<T>>? observers = null)
  {
    Value = initialValue;
    if (observers is not null)
      AddObservers(observers);
  }

  public void ChangeValueAndNotifyObservers(T value)
  {
    Value = value;
    NotifyObservers();
  }

  public void AddObserver(Action<T> observer)
    => this.observers.Add(observer);

  public void AddObservers(IEnumerable<Action<T>> observers)
    => this.observers.AddRange(observers);

  public void RemoveObserver(Action<T> observer)
    => observers.Remove(observer);

  public void NotifyObservers()
  {
    foreach (var observer in observers) observer(Value);
  }
}