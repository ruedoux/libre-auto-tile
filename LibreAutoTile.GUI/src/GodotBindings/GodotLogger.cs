using Godot;
using Qwaitumin.Logging;

namespace Qwaitumin.LibreAutoTile.GUI.GodotBindings;

public static class GodotLogger
{
  public static readonly Logging.Logger LOGGER = null!;

  static GodotLogger()
  {
    var logFilePath = Path.Combine(AppContext.BaseDirectory, "guilog.txt");
    if (OS.IsDebugBuild())
      logFilePath = "./guilog.txt";

    MessageFileWriter messageFileWriter = new(logFilePath);
    LOGGER = new([(msg) => GD.PrintRich(msg), messageFileWriter.Write], new(ColorType: ColorType.BBCODE));
    LOGGER.Log($"Logs are written to: '{logFilePath}'");
  }

  [System.Diagnostics.CodeAnalysis.DoesNotReturn]
  public static void LogErrorAndThrow(string message)
  {
    LOGGER.LogError(message);
    throw new ArgumentException(message);
  }
}