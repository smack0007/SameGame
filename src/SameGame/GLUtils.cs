using System;
using System.Collections.Generic;
using System.Text;
using static GLESDotNet.EGL;
using static GLESDotNet.GLES2;

namespace SameGame
{
    internal unsafe static class GLUtils
    {
        public static void CreateContext(IntPtr window, out IntPtr display, out IntPtr surface)
        {
            display = eglGetDisplay((IntPtr)EGL_DEFAULT_DISPLAY);

            int majorVersion, minorVersion;
            if (!eglInitialize(display, &majorVersion, &minorVersion))
            {
                int error = eglGetError();
                throw new InvalidOperationException();
            }

            eglBindAPI(EGL_OPENGL_ES_API);
            if (eglGetError() != EGL_SUCCESS)
            {
                throw new InvalidOperationException();
            }

            int[] configAttributes = new int[]
            {
                EGL_RED_SIZE, 8,
                EGL_GREEN_SIZE, 8,
                EGL_BLUE_SIZE, 8,
                EGL_ALPHA_SIZE, 8,
                EGL_DEPTH_SIZE, 24,
                EGL_STENCIL_SIZE, 8,
                EGL_SAMPLE_BUFFERS, EGL_DONT_CARE,
                EGL_NONE
            };

            IntPtr config;
            int configCount;
            fixed (int* configAttributesPtr = configAttributes)
            {
                if (!eglChooseConfig(display, configAttributesPtr, &config, 1, &configCount) || (configCount != 1))
                {
                    throw new InvalidOperationException();
                }
            }

            int[] surfaceAttributes = new int[]
            {
                EGL_NONE, EGL_NONE,
            };

            fixed (int* surfaceAttribtuesPtr = surfaceAttributes)
            {
                surface = eglCreateWindowSurface(display, config, GLFWUtils.GetNativeWindowHandle(window), surfaceAttribtuesPtr);
            }

            if (surface == IntPtr.Zero)
            {
                eglGetError(); // Clear error and try again
                surface = eglCreateWindowSurface(display, config, IntPtr.Zero, null);
            }

            if (eglGetError() != EGL_SUCCESS)
            {
                throw new InvalidOperationException();
            }

            int[] contextAttibutes = new int[]
            {
                EGL_CONTEXT_CLIENT_VERSION, 2,
                EGL_NONE
            };

            IntPtr context;
            fixed (int* contextAttributesPtr = contextAttibutes)
            {
                context = eglCreateContext(display, config, IntPtr.Zero, contextAttributesPtr);
                if (eglGetError() != EGL_SUCCESS)
                {
                    throw new InvalidOperationException();
                }
            }

            eglMakeCurrent(display, surface, surface, context);
            if (eglGetError() != EGL_SUCCESS)
            {
                throw new InvalidOperationException();
            }

            // Turn off vsync
            eglSwapInterval(display, 0);

            glInit(eglGetProcAddress);
        }

        public static uint CompileShader(string shaderSrc, uint type)
        {
            var shader = glCreateShader(type);

            if (shader == 0)
                return 0;

            var shaderSrcTmp = new string[] { shaderSrc };
            var shaderLength = shaderSrc.Length;
            glShaderSource(shader, 1, shaderSrcTmp, &shaderLength);

            glCompileShader(shader);

            int compiled;
            glGetShaderiv(shader, GL_COMPILE_STATUS, &compiled);

            if (compiled == 0)
            {
                int infoLength;
                glGetShaderiv(shader, GL_INFO_LOG_LENGTH, &infoLength);

                if (infoLength > 1)
                {
                    var infoLog = new StringBuilder(infoLength);
                    glGetShaderInfoLog(shader, infoLength, null, infoLog);
                    glDeleteShader(shader);
                    throw new InvalidOperationException($"Error compiling shader:\n{infoLog}");
                }
            }

            return shader;
        }

        public static void LinkProgram(uint program)
        {
            glLinkProgram(program);

            int linked;
            glGetProgramiv(program, GL_LINK_STATUS, &linked);

            if (linked == 0)
            {
                int infoLength;
                glGetProgramiv(program, GL_INFO_LOG_LENGTH, &infoLength);

                if (infoLength > 1)
                {
                    var infoLog = new StringBuilder(infoLength);
                    glGetProgramInfoLog(program, infoLength, null, infoLog);
                    glDeleteProgram(program);
                    throw new InvalidOperationException($"Error linking program:\n{infoLog}");
                }
            }
        }
    }
}
