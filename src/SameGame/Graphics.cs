using System;
using System.Data;
using System.Numerics;
using RaylibSharp;

namespace SameGame;

public class Graphics : IDisposable
{
    private readonly Game _game;

    public Graphics(Game game)
    {
        _game = game;
    }

    public void Dispose()
    {
    }

    public Texture LoadTexture(string fileName)
    {
        var handle = Raylib.LoadTexture(fileName);



        return new Texture(handle);
    }

    public void Initialize()
    {
    }

    public void BeginDraw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Raylib.BLACK);
    }

    public void EndDraw()
    {
        Raylib.EndDrawing();
    }

    public void Shutdown()
    {
    }

    public void DrawSprite(Texture texture, Vector2 pos, int srcX, int srcY, int srcWidth, int srcHeight, Vector4 color)
    {
        Raylib.DrawTexturePro(
            texture.Handle,
            new Raylib.Rectangle() { x = srcX, y = srcY, width = srcWidth, height = srcHeight },
            new Raylib.Rectangle() { x = pos.X, y = pos.Y, width = srcWidth, height = srcHeight },
            new Raylib.Vector2() { x = srcWidth / 2, y = srcHeight / 2 },
            0.0f,
            new Raylib.Color()
            {
                r = (byte)(255 * color.X),
                g = (byte)(255 * color.Y),
                b = (byte)(255 * color.Z),
                a = (byte)(255 * color.W),
            }
        );
    }
}
