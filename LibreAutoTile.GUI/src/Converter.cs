namespace Qwaitumin.LibreAutoTile.GUI;

public static class Converter
{
  public static string NullableToString(uint? number)
    => number == null ? "null" : number.Value.ToString();
}