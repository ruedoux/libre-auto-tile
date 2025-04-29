using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;

namespace Qwaitumin.Logging;

public record LogSettings(
  bool SupressError = false,
  bool SupressWarning = false,
  bool LogThread = false,
  bool Debug = false,
  uint ClassNameSize = 16,
  string TimeFormat = "HH:mm:ss.fff",
  uint ThreadIdSize = 8
);


public class Logger(IEnumerable<Action<Message>> observers, LogSettings logSettings)
{
  private const string NULL_STRING = "<null>";
  private static readonly object printLock = new();

  private readonly ObserverNotifier<Message> observerNotifier = new(observers);
  private readonly LogSettings logSettings = logSettings;
  private static readonly char[] separator = ['\r', '\n'];

  public static string ParseAsString(params object?[]? msgs)
  {
    List<object?> objects = [];
    if (msgs is null)
      objects.Add(null);
    else
      foreach (var msg in msgs)
        objects.Add(msg);

    var builder = new StringBuilder();
    builder.Append(string.Join("", objects.Select(ParseAsString)));
    return builder.ToString();
  }

  public void Log(string message, [CallerFilePath] string callerFilePath = "")
  {
    PushMessage(Message.GetInfo(
      GetSourceClassName(callerFilePath), message, logSettings));
  }

  public void Log(object message, [CallerFilePath] string callerFilePath = "")
  {
    PushMessage(Message.GetInfo(
      GetSourceClassName(callerFilePath), ParseAsString(message), logSettings));
  }

  public void Log(object?[] messages, [CallerFilePath] string callerFilePath = "")
  {
    var messageString = ParseAsString(messages);
    PushMessage(Message.GetInfo(
      GetSourceClassName(callerFilePath), messageString, logSettings));
  }

  public void LogError(string message, [CallerFilePath] string callerFilePath = "")
  {
    if (logSettings.SupressError) return;

    PushMessage(Message.GetError(
      GetSourceClassName(callerFilePath), message, logSettings));
  }

  public void LogWarning(string message, [CallerFilePath] string callerFilePath = "")
  {
    if (logSettings.SupressWarning) return;

    PushMessage(Message.GetWarning(
      GetSourceClassName(callerFilePath), message, logSettings));
  }

  public void LogDebug(string message, [CallerFilePath] string callerFilePath = "")
  {
    if (!logSettings.Debug) return;

    PushMessage(Message.GetDebug(
      GetSourceClassName(callerFilePath), message, logSettings));
  }

  public void LogException(string message, Exception ex, [CallerFilePath] string callerFilePath = "")
  {
    if (logSettings.SupressError) return;
    if (ex is null) return;

    string output = ex.Message;
    if (ex.InnerException is not null)
    {
      List<string> parsedExceptions = [ex.InnerException.Message];
      string[] lines = ex.InnerException.ToString()
        .Split(separator, StringSplitOptions.RemoveEmptyEntries);
      foreach (string line in lines) parsedExceptions.Add(line);
      output = message + "\n" + string.Join('\n', parsedExceptions.ToArray());
    }

    PushMessage(Message.GetError(
      GetSourceClassName(callerFilePath), output, logSettings));
  }

  // NOTE: This could probably delegate the notification to
  //       a different thread so it wont block printing, but
  //       to make sure it wont cause issues its locked. 
  private static void ForwardMessageToObservers(
    Message message, ObserverNotifier<Message> messageObservers)
  {
    lock (printLock)
    {
      messageObservers.NotifyObservers(message);
    }
  }

  private static string GetSourceClassName(string filePath)
    => Path.GetFileNameWithoutExtension(filePath) ?? "<Unknown>";

  private static string ParseAsString(object? msg)
  {
    if (msg is null)
      return NULL_STRING;

    if (msg is IDictionary dictionary)
    {
      var pairs = new List<string>();
      foreach (DictionaryEntry entry in dictionary)
        pairs.Add($"\"{ParseAsString(entry.Key)}\": {ParseAsString(entry.Value)}");
      return $"{{{string.Join(", ", pairs)}}}";
    }

    if (msg is ICollection collection)
    {
      var elements = new List<string>();
      foreach (var item in collection)
        elements.Add(ParseAsString(item));
      return $"[{string.Join(", ", elements)}]";
    }

    if (msg is IEnumerable enumerable and not string)
    {
      var elements = new List<string>();
      foreach (var item in enumerable)
        elements.Add(ParseAsString(item));
      return $"[{string.Join(", ", elements)}]";
    }

    return msg.ToString() ?? NULL_STRING;
  }

  private void PushMessage(Message message)
   => ForwardMessageToObservers(message, observerNotifier);

  private sealed class ObserverNotifier<T>
  {
    private readonly List<Action<T>> _observers = [];

    public ObserverNotifier(IEnumerable<Action<T>> observers)
      => AddObservers(observers);

    public void AddObservers(IEnumerable<Action<T>> observers)
      => _observers.AddRange(observers);

    public void NotifyObservers(T arg)
    {
      foreach (var observer in _observers) observer(arg);
    }
  }
}