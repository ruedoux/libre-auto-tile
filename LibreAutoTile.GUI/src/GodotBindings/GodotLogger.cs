using System;
using System.IO;
using Godot;
using Qwaitumin.Logging;

namespace Qwaitumin.LibreAutoTile.GUI.GodotBindings;

public static class GodotLogger
{
  public readonly static Logger Logger = null!;

  static GodotLogger()
  {
    var logFilePath = Path.Combine(AppContext.BaseDirectory, "guilog.txt");
    if (OS.IsDebugBuild())
      logFilePath = "./guilog.txt";

    MessageFileWriter messageFileWriter = new(logFilePath);
    Logger = new([(msg) => GD.PrintRich(msg), (msg) => messageFileWriter.Write(msg)], new(ColorType: ColorType.BBCODE));
    Logger.Log($"Logs are written to: '{logFilePath}'");
  }

  [System.Diagnostics.CodeAnalysis.DoesNotReturn]
  public static void LogErrorAndThrow(string message)
  {
    Logger.LogError(message);
    throw new ArgumentException(message);
  }
}