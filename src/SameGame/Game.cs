using System;
using System.Diagnostics;
using SameGame.Logic;
using SameGame.Rendering;
using RaylibSharp;
using System.Runtime.InteropServices.Marshalling;

namespace SameGame
{
    public class Game : IDisposable
    {
        public int WindowWidth { get; } = 1024;

        public int WindowHeight { get; } = 768;

        public Graphics Graphics { get; private set; }

        public BoardRenderer BoardRenderer { get; private set; }

        public Board Board { get; private set; }

        public Game()
        {
            Raylib.InitWindow(WindowWidth, WindowHeight, "SameGame");
            Raylib.SetTargetFPS(60);

            Graphics = new Graphics(this);

            BoardRenderer = new BoardRenderer(Graphics);

            Board = new Board(new RNG(100));
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
            Graphics.Initialize();
            BoardRenderer.Initialize();
        }

        private void Update(float elapsed)
        {
            var mousePosition = Raylib.GetMousePosition();

            int x = (int)mousePosition.x / BoardRenderer.BlockWidth;
            int y = (int)mousePosition.y / BoardRenderer.BlockHeight;

            if (Raylib.IsMouseButtonPressed(Raylib.MOUSE_BUTTON_LEFT))
            {
                Board.LeftClick(x, y);
            }

            Board.Update(elapsed);
        }

        private void Draw()
        {
            Graphics.BeginDraw();
            BoardRenderer.Render(Board);
            Graphics.EndDraw();
        }

        private void Shutdown()
        {
            BoardRenderer.Shutdown();
            Graphics.Shutdown();
        }
    }
}
