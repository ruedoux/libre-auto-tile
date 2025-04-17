using Godot;
using Qwaitumin.AutoTile.Configuration;

namespace Qwaitumin.AutoTile.GUI.Core.GodotBindings;

public static class GodotTypeMapper
{
  public static Godot.Vector2I Map(Configuration.Vector2 v)
    => new(v.X, v.Y);

  public static Godot.Vector3I Map(Configuration.Vector3 v)
    => new(v.X, v.Y, v.Z);

  public static Configuration.Vector2 Map(Godot.Vector2I v)
    => new(v.X, v.Y);

  public static Configuration.Vector3 Map(Godot.Vector3I v)
    => new(v.X, v.Y, v.Z);

  public static Configuration.Vector2 Map(Godot.Vector2 v)
    => new((int)v.X, (int)v.Y);

  public static Configuration.Vector3 Map(Godot.Vector3 v)
    => new((int)v.X, (int)v.Y, (int)v.Z);

  public static TileColor Map(Color color)
    => new((byte)color.R8, (byte)color.G8, (byte)color.B8, (byte)color.A8);

  public static Color Map(TileColor color)
    => new(color.Rf, color.Gf, color.Bf, color.Af);
}