using System;
using System.Collections.Generic;

namespace Qwaitumin.LibreAutoTile.GUI.Core.Signals;

public class EventNotifier<T>
{
  private readonly List<Action<T>> observers = [];
  private readonly object lockObject = new();

  public EventNotifier() { }

  public EventNotifier(IEnumerable<Action<T>> observers)
    => AddObservers(observers);

  public void AddObserver(Action<T> observer)
  {
    lock (lockObject)
      observers.Add(observer);
  }

  public void AddObservers(IEnumerable<Action<T>> observers)
  {
    lock (lockObject)
      this.observers.AddRange(observers);
  }

  public void RemoveObserver(Action<T> observer)
  {
    lock (lockObject)
      observers.Remove(observer);
  }

  public void NotifyObservers(T arg)
  {
    lock (lockObject)
    {
      foreach (var observer in observers)
        observer(arg);
    }
  }
}