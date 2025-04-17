using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace Qwaitumin.AutoTile.GUI.Scenes.Editor.Utils;

public partial class MessageDisplay : MarginContainer
{
  public RichTextLabel RichTextLabel = null!;
  private CancellationTokenSource? currentFadeCts;

  public override void _Ready()
  {
    RichTextLabel = GetNode<RichTextLabel>("RichTextLabel");
    RichTextLabel.Modulate = new Color(1, 1, 1, 0);
  }

  public void DisplayText(string text, int holdMs = 3000, int fadeMs = 2000)
  {
    RichTextLabel.Text = text;

    if (currentFadeCts != null)
    {
      currentFadeCts.Cancel();
      currentFadeCts.Dispose();
    }

    currentFadeCts = new CancellationTokenSource();
    _ = TextFadeEffect(holdMs, fadeMs, currentFadeCts.Token);
  }

  private async Task TextFadeEffect(
    int holdMs,
    int fadeMs,
    CancellationToken token,
    int updateIntervalMs = 10)
  {
    if (updateIntervalMs <= 0)
      throw new ArgumentException($"Update interval must be bigger than 0 but is: {updateIntervalMs}");

    RichTextLabel.Modulate = new Color(1, 1, 1, 1);
    await Task.Delay(holdMs, token);
    Stopwatch stopwatch = Stopwatch.StartNew();
    while (stopwatch.ElapsedMilliseconds < fadeMs)
    {
      if (token.IsCancellationRequested)
        return;

      float elapsed = stopwatch.ElapsedMilliseconds;
      float progress = Math.Min(elapsed / fadeMs, 1f);
      RichTextLabel.Modulate = new Color(1, 1, 1, 1 - progress);

      await Task.Delay(updateIntervalMs, token);
    }

    RichTextLabel.Modulate = new Color(1, 1, 1, 0);
    if (currentFadeCts != null && token == currentFadeCts.Token)
    {
      currentFadeCts.Dispose();
      currentFadeCts = null;
    }
  }

  public override void _ExitTree()
  {
    currentFadeCts?.Cancel();
    currentFadeCts?.Dispose();
    currentFadeCts = null;
    base._ExitTree();
  }
}
