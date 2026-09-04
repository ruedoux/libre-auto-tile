using Godot;
using System.Threading;

namespace LibreAutoTile.GodotBindings.Tests;

public class GodotAccess
{
  private static SceneTree sceneTree = null!;
  private static Node2D accessNode = null!;


  public static void Bind(SceneTree sceneTree)
  {
    GodotAccess.sceneTree = sceneTree;
    GodotAccess.accessNode = new();
    Callable.From(() => sceneTree.Root.AddChild(accessNode)).CallDeferred();
  }

  public static void AddNodeToTree(Node node)
  {
    Callable.From(() => accessNode.AddChild(node)).CallDeferred();
    WaitNextFrames();
  }

  /// <summary>
  /// Runs a function on the main thread and blocks until it completes
  /// </summary>
  public static T RunOnMainThread<T>(Func<T> func)
  {
    T result = default!;
    Exception? error = null;
    using var done = new ManualResetEventSlim(false);
    Callable.From(() =>
    {
      try
      {
        result = func();
      }
      catch (Exception e)
      {
        error = e;
      }
      finally
      {
        done.Set();
      }
    }).CallDeferred();
    done.Wait();
    if (error is not null)
      throw error;
    return result;
  }

  public static void WaitNextFrames(int n = 2)
  {
    for (int i = 0; i < n; i++)
      WaitNextFrameAsync().Wait();
  }

  private static async Task WaitNextFrameAsync()
    => await sceneTree.ToSignal(sceneTree, SceneTree.SignalName.ProcessFrame);
}
