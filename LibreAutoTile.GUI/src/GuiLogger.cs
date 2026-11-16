using System.Runtime.CompilerServices;
using SimpleLogging;

namespace LibreAutoTile.GUI;

public class GuiLogger
{
  private readonly Logger logger;

  public GuiLogger(Action<string> messageObserver, string logFilePath, ColorType colorType = ColorType.BBCODE)
  {
    Action<Message>[] listeners = [(msg) => messageObserver.Invoke(msg.ToString()), new MessageFileWriter(logFilePath).Write];
    logger = new(listeners, new(ColorType: colorType));
    logger.Log($"Logs are written to: '{logFilePath}'");
  }

  public void Log(object message, [CallerFilePath] string callerFilePath = "")
    => logger.Log(message, callerFilePath);

  public void Log(object?[] messages, [CallerFilePath] string callerFilePath = "")
    => logger.Log(messages, callerFilePath);

  public void LogError(string message, [CallerFilePath] string callerFilePath = "")
    => logger.LogError(message, callerFilePath);

  public void LogWarning(string message, [CallerFilePath] string callerFilePath = "")
    => logger.LogWarning(message, callerFilePath);

  public void LogDebug(string message, [CallerFilePath] string callerFilePath = "")
    => logger.LogDebug(message, callerFilePath);

  public void LogException(string message, Exception ex, [CallerFilePath] string callerFilePath = "")
    => logger.LogException(message, ex, callerFilePath);
}
