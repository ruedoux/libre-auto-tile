using Godot;
using Qwaitumin.SimpleTest;

namespace LibreAutoTile.GodotBindings.Tests;

public partial class Run : Node2D
{
  Task task = null!;

  public override void _Ready()
  {
    GodotAccess.Bind(GetTree());
    var args = OS.GetCmdlineArgs()
      .Where(arg => arg.Contains("--test-method") || arg.Contains("--test-method"))
      .ToArray();

    task = Task.Run(() =>
    {
      if (!new SimpleTestPrinter(Console.WriteLine).Run(args))
        System.Environment.Exit(1);
    });
  }

  public override void _PhysicsProcess(double delta)
  {
    if (task.IsCompleted)
      GetTree().Quit();
  }
}
