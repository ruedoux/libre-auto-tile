using Godot;

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

  public static void WaitNextFrames(int n = 2)
  {
    for (int i = 0; i < n; i++)
      WaitNextFrameAsync().Wait();
  }

  private static async Task WaitNextFrameAsync()
    => await sceneTree.ToSignal(sceneTree, SceneTree.SignalName.ProcessFrame);
}