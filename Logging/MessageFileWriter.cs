namespace Qwaitumin.Logging;

public class MessageFileWriter
{
  private readonly string filePath;

  public MessageFileWriter(string filePath)
  {
    this.filePath = filePath;
    if (File.Exists(filePath))
      File.Delete(filePath);
  }

  public void Write(Message message)
  {
    using StreamWriter writer = new(filePath, true);
    writer.WriteLine(message.GetAsString(false));
  }
}