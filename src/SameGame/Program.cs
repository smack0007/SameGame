using System;
using System.IO;
using System.Reflection;
using static GLESDotNet.GLES2;
using static GLFWDotNet.GLFW;

namespace SameGame
{
    class Program
    {
        static void Main()
        {
            using var game = new Game();
            game.Run();
        }
    }
}
