using System.Reflection;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Qwaitumin.SimpleTest;

public enum Result { SUCCESS, FAIL }

[AttributeUsage(AttributeTargets.Class)]
public class SimpleTestClass : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public class SimpleTestMethod : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public class SimpleBeforeEach : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public class SimpleAfterEach : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public class SimpleBeforeAll : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public class SimpleAfterAll : Attribute { }


public record SimpleTestMethodResult(string Name, Result Result, string[] Messages);

public record SimpleTestClassResult(
  string Name,
  Result Result,
  List<SimpleTestMethodResult> MethodResults,
  long TookMiliseconds,
  string[] Messages);

// This really doesnt matter since tests will not be included in the release build,
// or it can simply be marked to not trim test classes.
#pragma warning disable IL2067
#pragma warning disable IL2026
#pragma warning disable IL2070
public class SimpleTestRunner
{
  public List<SimpleTestClassResult> ClassResults = new();

  private readonly Action<Type>? beginPrinterMethod;
  private readonly Action<SimpleTestClassResult>? resultPrinterMethod;

  public SimpleTestRunner(
    Action<Type>? beginPrinterMethod = null,
    Action<SimpleTestClassResult>? resultPrinterMethod = null)
  {
    this.beginPrinterMethod = beginPrinterMethod;
    this.resultPrinterMethod = resultPrinterMethod;
  }

  public void RunAll()
  {
    var testTypes = GetTestAllClassTypes();

    foreach (var testType in testTypes)
      RunForType(testType);
  }

  public SimpleTestClassResult RunForType(Type testClassType)
  {
    if (!testClassType.IsDefined(typeof(SimpleTestClass), inherit: true))
      throw new ArgumentException($"Passed class: '{testClassType.Name}' is not of '{nameof(SimpleTestClass)}' class type");
    beginPrinterMethod?.Invoke(testClassType);

    var testObject = Activator.CreateInstance(testClassType);
    var beforeAllMethod = GetMethodWithAttribute<SimpleBeforeAll>(testClassType);
    var afterAllMethod = GetMethodWithAttribute<SimpleAfterAll>(testClassType);
    var beforeEachMethod = GetMethodWithAttribute<SimpleBeforeEach>(testClassType);
    var afterEachMethod = GetMethodWithAttribute<SimpleAfterEach>(testClassType);
    var testMethods = GetMethodsWithAttribute<SimpleTestMethod>(testClassType);

    List<SimpleTestMethodResult> methodResults = new();
    string[] exceptionMessages = Array.Empty<string>();
    Stopwatch classStopwatch = Stopwatch.StartNew();
    try
    {
      beforeAllMethod?.Invoke(testObject, null);
      foreach (var testMethod in testMethods)
      {
        bool methodFailed = false;
        try
        {
          beforeEachMethod?.Invoke(testObject, null);
          testMethod.Invoke(testObject, null);
          afterEachMethod?.Invoke(testObject, null);
        }
        catch (Exception ex)
        {
          methodFailed = true;
          methodResults.Add(new(
            testMethod.Name,
            Result.FAIL,
            ConvertExceptionToStringArray(ex)));
        }

        if (!methodFailed)
          methodResults.Add(new(
            testMethod.Name,
            Result.SUCCESS,
            Array.Empty<string>()));
      }
      afterAllMethod?.Invoke(testObject, null);
    }
    catch (Exception ex)
    {
      exceptionMessages = ConvertExceptionToStringArray(ex);
    }
    classStopwatch.Stop();

    var result = Result.SUCCESS;
    string[] messages = new string[] { "All methods succeeded" };
    if (exceptionMessages.Length != 0)
    {
      result = Result.FAIL;
      messages = exceptionMessages;
    }
    else if (methodResults.Exists(methodResult => methodResult.Result == Result.FAIL))
    {
      result = Result.FAIL;
      messages = new string[] { "At least one of the methods has failed" };
    }

    SimpleTestClassResult simpleTestClassResult = new(
      testClassType.Name, result, methodResults, classStopwatch.ElapsedMilliseconds, messages);

    ClassResults.Add(simpleTestClassResult);
    resultPrinterMethod?.Invoke(simpleTestClassResult);
    return simpleTestClassResult;
  }

  public static Type[] GetTestAllClassTypes()
  {
    var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
    List<Type> types = new();
    foreach (var assembly in allAssemblies)
      types.AddRange(assembly.GetTypes()
        .Where(t => t.GetCustomAttributes(typeof(SimpleTestClass), true).Length > 0));

    return types.ToArray();
  }

  private static MethodInfo? GetMethodWithAttribute<T>(Type type) where T : Attribute
    => Array.Find(type.GetMethods(), m => m.GetCustomAttributes(typeof(T), true).Length > 0);

  private static MethodInfo[] GetMethodsWithAttribute<T>(Type type) where T : Attribute
    => type.GetMethods()
      .Where(m => m.GetCustomAttributes(typeof(T), true).Length > 0)
      .ToArray();

  private static string[] ConvertExceptionToStringArray(Exception ex)
    => ex.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None);
}
#pragma warning restore IL2067
#pragma warning restore IL2026
#pragma warning restore IL2070

public static class SimpleEqualsVerifier
{
  public static void Verify<T>(
    T obj, T objToEqual, T objToNotEqual)
    where T : notnull
  {
    Assertions.AssertEqual(obj, objToEqual);
    Assertions.AssertNotEqual(obj, objToNotEqual);
  }
}

public sealed class SimpleTestDirectory : IDisposable
{
  public readonly string AbsolutePath;

  public SimpleTestDirectory(string path = "./TEMPORARY_TEST_FOLDER")
  {
    AbsolutePath = Path.GetFullPath(path);
    Create();
  }

  public string GetRelativePath(string path)
    => $"{AbsolutePath}/{path}";

  public void Clean()
  {
    Delete();
    Create();
  }

  public void Delete()
    => Directory.Delete(AbsolutePath, recursive: true);

  private void Create()
    => Directory.CreateDirectory(AbsolutePath);

  public void Dispose()
  {
    Delete();
  }
}

public class SimpleTestPrinter
{
  static class Ansi
  {
    public static readonly string NORMAL = Console.IsOutputRedirected ? "" : "\x1b[39m";
    public static readonly string RED = Console.IsOutputRedirected ? "" : "\x1b[91m";
    public static readonly string GREEN = Console.IsOutputRedirected ? "" : "\x1b[92m";
    public static readonly string BLUE = Console.IsOutputRedirected ? "" : "\x1b[94m";
    public static readonly string GREY = Console.IsOutputRedirected ? "" : "\x1b[97m";
  }

  static readonly string PREFIX_RUN = "[RUN]";
  static readonly string PREFIX_OK = "[OK ]";
  static readonly string PREFIX_ERROR = "[ERR]";

  private readonly Action<string> printFunction;

  public SimpleTestPrinter(Action<string> printFunction)
  {
    this.printFunction = printFunction;
  }

  public void Run()
  {
    SimpleTestRunner simpleTestRunner = new(LogClassBegin, LogClassResult);
    Stopwatch stopwatch = Stopwatch.StartNew();
    simpleTestRunner.RunAll();
    stopwatch.Stop();

    int classesPassed = 0, classesTotal = 0, methodsPassed = 0, methodsTotal = 0;
    foreach (var classResult in simpleTestRunner.ClassResults)
    {
      classesTotal++;
      if (classResult.Result == Result.SUCCESS) classesPassed++;
      foreach (var methodResult in classResult.MethodResults)
      {
        methodsTotal++;
        if (methodResult.Result == Result.SUCCESS) methodsPassed++;
      }
    }

    string resultText = classesTotal == classesPassed ? AddColorToString("PASS", Ansi.GREEN) : AddColorToString("FAIL", Ansi.RED);
    printFunction($"""
    ----------------------
    {resultText} Classes: {classesPassed}/{classesTotal} Methods: {methodsPassed}/{methodsTotal}
    Took {FormatTime(stopwatch.ElapsedMilliseconds)}
    ----------------------
    """);
  }

  private void LogClassBegin(Type type)
    => printFunction($"{AddColorToString(PREFIX_RUN, Ansi.BLUE)} {type.Name}");

  private void LogClassResult(SimpleTestClassResult classResult)
  {
    if (classResult.Result == Result.SUCCESS)
      printFunction($"{AddColorToString(PREFIX_OK, Ansi.GREEN)} {classResult.Name} {AddColorToString(FormatTime(classResult.TookMiliseconds), Ansi.GREY)}");
    if (classResult.Result == Result.FAIL)
      printFunction($"{AddColorToString(PREFIX_ERROR, Ansi.RED)} {classResult.Name} {AddColorToString(FormatTime(classResult.TookMiliseconds), Ansi.GREY)}");

    foreach (var methodResult in classResult.MethodResults)
      LogMethodResult(methodResult);
  }

  private void LogMethodResult(SimpleTestMethodResult methodResult)
  {
    if (methodResult.Result == Result.SUCCESS)
      printFunction($"-> {AddColorToString(PREFIX_OK, Ansi.GREEN)} {methodResult.Name}");
    if (methodResult.Result == Result.FAIL)
    {
      printFunction($"->{AddColorToString(PREFIX_ERROR, Ansi.RED)} {methodResult.Name}");
      printFunction(string.Join('\n', methodResult.Messages));
    }
  }

  private static string FormatTime(long miliseconds)
    => $"{miliseconds / 1000}.{miliseconds % 1000}s";

  private static string AddColorToString(string msg, string color)
    => $"{color}{msg}{Ansi.NORMAL}";
}


public static class Assertions
{
  public static void AssertFileExists(string filePath)
  {
    if (!File.Exists(filePath))
      throw new FileNotFoundException($"File doesnt exist: {filePath}");
  }

  public static void AssertDirectoryExists(string directoryPath)
  {
    if (!Directory.Exists(directoryPath))
      throw new DirectoryNotFoundException($"Directory doesnt exist: {directoryPath}");
  }

  public static void AssertNotNull<T>([NotNull] T? obj, string additionalMessage = "")
  {
    if (obj is null)
      throw new ArgumentNullException(
        nameof(obj), $"Argument cannot be null. {additionalMessage}");
  }

  public static void AssertNull<T>(T? obj, string additionalMessage = "")
  {
    if (obj is not null)
      throw new ArgumentNullException(
        nameof(obj), $"Argument should be null. {additionalMessage}");
  }

  public static void AssertTrue(bool shoudlBeTrue, string additionalMessage = "")
  {
    if (!shoudlBeTrue)
    {
      throw new ValidationException(
        $"Value is false, but expected true. {additionalMessage}");
    }
  }

  public static void AssertFalse(bool shoudlBeFalse, string additionalMessage = "")
  {
    if (shoudlBeFalse)
    {
      throw new ValidationException(
        $"Value is true, but expected false. {additionalMessage}");
    }
  }

  public static void AssertEqual<T>(
    T shouldBe, T isNow, string additionalMessage = "")
  {
    if (!Equals(shouldBe, isNow))
    {
      throw new ValidationException(
        $"Value is not equal, is: '{isNow}', but should be: '{shouldBe}'. {additionalMessage}");
    }
  }

  public static void AssertEqual<T>(
    IEnumerable<T> shouldBe, IEnumerable<T> isNow, string additionalMessage = "")
  {
    if (!Equals(shouldBe, isNow))
    {
      throw new ValidationException(
        $"Value is not equal, is: '{isNow}', but should be: '{shouldBe}'. {additionalMessage}");
    }
  }

  public static void AssertNotEqual<T>(
    T shouldNotBe, T isNow, string additionalMessage = "")
  {
    if (Equals(shouldNotBe, isNow))
    {
      throw new ValidationException(
        $"Value is equal to: '{shouldNotBe}'. {additionalMessage}");
    }
  }

  public static void AssertLessThan<T>(
    T value, T maxValue, string additionalMessage = "")
        where T : IComparable<T>
  {
    if (value.CompareTo(maxValue) >= 0)
    {
      throw new ValidationException(
          $"Value '{value}' is not less than '{maxValue}'. {additionalMessage}");
    }
  }

  public static void AssertMoreThan<T>(
    T value, T minValue, string additionalMessage = "")
      where T : IComparable<T>
  {
    if (value.CompareTo(minValue) <= 0)
    {
      throw new ValidationException(
          $"Value '{value}' is not larger than '{minValue}'. {additionalMessage}");
    }
  }

  public static void AssertEqualOrLessThan<T>(
    T value, T maxValue, string additionalMessage = "")
        where T : IComparable<T>
  {
    if (value.CompareTo(maxValue) > 0)
    {
      throw new ValidationException(
          $"Value '{value}' is greater than '{maxValue}'. {additionalMessage}");
    }
  }

  public static void AssertEqualOrMoreThan<T>(
    T value, T minValue, string additionalMessage = "")
      where T : IComparable<T>
  {
    if (value.CompareTo(minValue) < 0)
    {
      throw new ValidationException(
          $"Value '{value}' is less than '{minValue}'. {additionalMessage}");
    }
  }

  public static void AssertNotInRange<T>(
    T value, T minValue, T maxValue, string additionalMessage = "")
      where T : IComparable<T>
  {
    if (value.CompareTo(minValue) >= 0 && value.CompareTo(maxValue) <= 0)
    {
      throw new ValidationException(
          $"Value '{value}' is in range: '{minValue}' - '{maxValue}'. {additionalMessage}");
    }
  }

  public static void AssertInRange<T>(
    T value, T minValue, T maxValue, string additionalMessage = "")
      where T : IComparable<T>
  {
    if (value.CompareTo(minValue) < 0 || value.CompareTo(maxValue) > 0)
    {
      throw new ValidationException(
          $"Value '{value}' is not in range: '{minValue}' - '{maxValue}'. {additionalMessage}");
    }
  }

  public static void AssertAwaitAtMost(long timeoutMs, Action action)
  {
    Exception trackedException = new("Empty exception");
    var actionTask = Task.Run(() =>
    {
      while (true)
      {
        try
        {
          action();
          break;
        }
        catch (Exception ex)
        {
          trackedException = ex;
        }
        Thread.Sleep(10);
      }
    });

    var timeoutTask = Task.Delay(TimeSpan.FromMilliseconds(timeoutMs));

    if (Task.WhenAny(actionTask, timeoutTask).Result == timeoutTask)
      throw new TimeoutException(
        $"Assertion was not passed in time: {timeoutMs}ms.\n" +
        $"Reason: {trackedException.Message}\n" +
        $"{trackedException.StackTrace}");

    if (actionTask.IsFaulted && actionTask.Exception != null)
      throw actionTask.Exception;
  }

  public static void AssertThrows<T>(
    Action action, string additionalMessage = "") where T : Exception
  {
    try
    {
      action();
    }
    catch (T)
    {
      return;
    }
    catch (Exception ex)
    {
      throw new ValidationException(
        $"Expected exception of type '{typeof(T)}', but got '{ex.GetType()}' instead. {additionalMessage}");
    }

    throw new ValidationException($"Expected exception of type '{typeof(T)}' was not thrown. {additionalMessage}");
  }

}