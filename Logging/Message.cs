using System.Text;

namespace Qwaitumin.Logging;

public readonly struct Message
{
  public enum Level { INFO, WARN, ERROR, DEBUG }
  private static readonly Dictionary<Level, string> typeColorDict = new()
  { {Level.INFO, "Deepskyblue"}, {Level.WARN, "Orange"}, {Level.ERROR, "Red"}, {Level.DEBUG, "Purple"}};

  public readonly Level Type;
  public readonly string TypeStr;
  public readonly string Time;
  public readonly string ClassName;
  public readonly string ThreadId;
  public readonly string Text;
  public readonly bool LogThread;

  private Message(Level type, string className, string text, LogSettings logSettings)
  {
    Time = DateTime.Now.ToString(logSettings.TimeFormat);
    ThreadId = Environment.CurrentManagedThreadId.ToString("X")
      .PadLeft((int)logSettings.ThreadIdSize)[..(int)logSettings.ThreadIdSize];
    TypeStr = type.ToString().PadLeft(5)[..5];
    Type = type;
    ClassName = className.PadLeft((int)logSettings.ClassNameSize)[..(int)logSettings.ClassNameSize];
    LogThread = logSettings.LogThread;
    Text = text;
  }

  public string GetAsString(bool withBBCode, bool withContext = true)
  {
    var builder = new StringBuilder();
    if (withContext)
      builder.Append(GetContext(withBBCode)).Append(" : ");
    builder.Append(Text);
    return builder.ToString();
  }

  public override string ToString()
    => GetAsString(false);

  private string GetContext(bool withBBCode)
  {
    string finalTypeStr = TypeStr;
    if (withBBCode)
      finalTypeStr = AddColorToString(finalTypeStr, typeColorDict[Type]);

    var contextBuilder = new StringBuilder();
    contextBuilder.Append($"{Time} {finalTypeStr} [{ClassName}]");
    if (LogThread)
      contextBuilder.Append($" [{ThreadId}]");

    return contextBuilder.ToString();
  }

  private static string AddColorToString(string msg, string color)
    => $"[color={color}]{msg}[/color]";

  public static Message GetInfo(string className, string text, LogSettings logSettings)
    => new(Level.INFO, className, text, logSettings);
  public static Message GetWarning(string className, string text, LogSettings logSettings)
    => new(Level.WARN, className, text, logSettings);
  public static Message GetError(string className, string text, LogSettings logSettings)
    => new(Level.ERROR, className, text, logSettings);
  public static Message GetDebug(string className, string text, LogSettings logSettings)
    => new(Level.DEBUG, className, text, logSettings);
}