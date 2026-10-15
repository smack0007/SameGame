using System;
using SameGame.Gameplay;
using RaylibSharp;

namespace SameGame;

public class Game : IDisposable
{
    public const int WindowWidth = 1024;
    public const int WindowHeight = 768;

    public Graphics _graphics;
    public Mouse _mouse;
    public Board _board;

    public Game()
    {
        Raylib.InitWindow(WindowWidth, WindowHeight, "SameGame");
        Raylib.SetTargetFPS(60);

        _graphics = new Graphics(this);
        _mouse = new Mouse();
        _board = new Board(_graphics, new RNG(100));
    }

    public void Dispose()
    {
        Raylib.CloseWindow();
    }

    public void Run()
    {
        Initialize();

        while (!Raylib.WindowShouldClose() && !Raylib.IsKeyDown(Raylib.KEY_ESCAPE))
        {
            Tick();
        }

        Shutdown();
    }

    private void Tick()
    {
        // raylib's elapsed time is in seconds so convert it to ms.
        var elapsedTime = Raylib.GetFrameTime() * 1000.0f;
        Update(elapsedTime);
        Draw();
    }

    private void Initialize()
    {
        _graphics.Initialize();
        _board.Initialize();
    }

    private void Shutdown()
    {
        _board.Shutdown();
        _graphics.Shutdown();
    }

    private void Update(float elapsed)
    {
        _mouse.Update();
        _board.Update(elapsed, _mouse);
    }

    private void Draw()
    {
        _graphics.BeginDraw();
        _board.Draw();
        _graphics.EndDraw();
    }
}
