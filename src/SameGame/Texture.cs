using RaylibSharp;

namespace SameGame;

public readonly struct Texture
{
    public static readonly Texture None = new Texture(new Raylib.Texture() { id = 0 });

    public Raylib.Texture Handle { get; }

    public int Width => Handle.width;

    public int Height => Handle.height;

    public Texture(Raylib.Texture handle)
    {
        Handle = handle;
    }
}

