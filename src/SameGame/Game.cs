﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using SameGame.Logic;
using SameGame.Rendering;
using static GLFWDotNet.GLFW;
using static GLESDotNet.EGL;

namespace SameGame
{
    public class Game : IDisposable
    {
        private const float TimeBetweenFrames = 1000.0f / 60.0f;

        private IntPtr _window;
        private GLFWwindowsizefun _windowSizeCallback;
        private GLFWkeyfun _keyboardCallback;

        private IntPtr _display;
        private IntPtr _surface;

        private Stopwatch _stopwatch = new Stopwatch();
        private float _lastElapsed;
        private float _elapsedSinceLastFrame;

        private float _fpsElapsed;

        public int WindowWidth { get; private set; } = 1024;

        public int WindowHeight { get; private set; } = 768;

        public int FramesPerSecond { get; private set; }

        public Graphics Graphics { get; private set; }

        public BoardRenderer BoardRenderer { get; private set; }

        public Board Board { get; private set; }

        public Game()
        {
            var basePath = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location ?? "");
            Directory.SetCurrentDirectory(basePath);

            if (glfwInit() == 0)
                throw new InvalidOperationException("Failed to initialize GLFW.");

            glfwWindowHint(GLFW_CLIENT_API, GLFW_NO_API);

            _window = glfwCreateWindow(1024, 768, "SameGame", IntPtr.Zero, IntPtr.Zero);

            if (_window == IntPtr.Zero)
                throw new InvalidOperationException("Failed to create window.");

            _windowSizeCallback = OnWindowSize;
            glfwSetWindowSizeCallback(_window, _windowSizeCallback);

            _keyboardCallback = OnKeyboard;
            glfwSetKeyCallback(_window, _keyboardCallback);

            GLUtils.CreateContext(_window, out _display, out _surface);

            Graphics = new Graphics(this);

            BoardRenderer = new BoardRenderer(Graphics);

            Board = new Board(new Random());
        }

        public void Dispose()
        {
            Graphics.Dispose();
            glfwTerminate();
        }

        protected virtual void OnWindowSize(IntPtr window, int width, int height)
        {
            WindowWidth = width;
            WindowHeight = height;
        }

        protected virtual void OnKeyboard(IntPtr window, int key, int scancode, int action, int mods)
        {
            if (key == GLFW_KEY_ESCAPE)
                glfwSetWindowShouldClose(_window, 1);
        }

        public void Run()
        {
            _stopwatch.Restart();

            Initialize();

            while (glfwWindowShouldClose(_window) == 0)
            {
                glfwPollEvents();

                Tick();
            }
        }

        private void Tick()
        {
            float currentElapsed = (float)_stopwatch.Elapsed.TotalMilliseconds;
            float deltaElapsed = currentElapsed - _lastElapsed;
            _elapsedSinceLastFrame += deltaElapsed;
            _lastElapsed = currentElapsed;

            bool shouldDraw = _elapsedSinceLastFrame >= TimeBetweenFrames;

            while (_elapsedSinceLastFrame >= TimeBetweenFrames)
            {
                Update(_elapsedSinceLastFrame);
                _elapsedSinceLastFrame -= TimeBetweenFrames;
            }

            if (shouldDraw)
            {
                Draw();
                eglSwapBuffers(_display, _surface);
                FramesPerSecond++;
            }

            _fpsElapsed += deltaElapsed;

            if (_fpsElapsed >= 1000.0f)
            {
                _fpsElapsed -= 1000.0f;
                FramesPerSecond = 0;
            }
        }

        private void Initialize()
        {
            Graphics.Initialize();

            BoardRenderer.Initialize();
        }

        private void Update(float elapsed)
        {
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
