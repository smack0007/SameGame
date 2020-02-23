using System;
using System.Runtime.InteropServices;
using static GLFWDotNet.GLFW;

namespace SameGame
{
    internal static class GLFWUtils
    {
        public static IntPtr GetNativeWindowHandle(IntPtr window)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return glfwGetWin32Window(window);

            throw new NotImplementedException($"{nameof(GetNativeWindowHandle)} not implemented on this platform.");
        }
    }
}
