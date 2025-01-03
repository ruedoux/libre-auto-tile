using System;
using System.Collections.Generic;

namespace Qwaitumin.AutoTile.GUI.Core.Signals;

public class ObserverNotifier<T>
{
  private readonly List<Action<T>> observers = [];

  public ObserverNotifier() { }

  public ObserverNotifier(IEnumerable<Action<T>> observers)
    => AddObservers(observers);

  public void AddObserver(Action<T> observer)
    => observers.Add(observer);

  public void AddObservers(IEnumerable<Action<T>> observers)
    => this.observers.AddRange(observers);

  public void RemoveObserver(Action<T> observer)
    => observers.Remove(observer);

  public void NotifyObservers(T arg)
  {
    foreach (var observer in observers) observer(arg);
  }
}