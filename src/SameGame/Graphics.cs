using System;
using System.Numerics;
using ImageDotNet;
using static GLESDotNet.GLES2;

namespace SameGame
{
    public unsafe class Graphics : IDisposable
    {
        private readonly Game _game;

        private uint _program;
        private int _vertTransformLocation;
        private int _fragTextureLocation;

        private const int MaxSpriteCount = 1024;

        private int _vertCount = 0;
        private Vector3[] _vertPositions;
        private Vector4[] _vertColors;
        private Vector2[] _vertTexCoords;

        private int _indexCount = 0;
        private ushort[] _indices;

        private Texture _texture = Texture.None;

        public Graphics(Game game)
        {
            _game = game;

            _vertPositions = new Vector3[MaxSpriteCount * 4];
            _vertColors = new Vector4[MaxSpriteCount * 4];
            _vertTexCoords = new Vector2[MaxSpriteCount * 4];
            _indices = new ushort[MaxSpriteCount * 6];
        }

        public void Dispose()
        {
        }

        public Texture LoadTexture(string fileName)
        {
            uint handle;
            glGenTextures(1, &handle);

            glActiveTexture(GL_TEXTURE0);
            glBindTexture(GL_TEXTURE_2D, handle);

            Image<Rgba32> image = Image.LoadPng(fileName).To<Rgba32>();
            using (ImageDataPointer data = image.GetDataPointer())
            {
                glTexImage2D(GL_TEXTURE_2D, 0, (int)GL_RGBA, image.Width, image.Height, 0, GL_RGBA, GL_UNSIGNED_BYTE, (void*)data.Pointer);
            }

            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, (int)GL_LINEAR);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, (int)GL_LINEAR);

            return new Texture(handle, image.Width, image.Height);
        }

        public void Initialize()
        {
            string vertShader =
@"attribute vec3 vertPosition;
attribute vec4 vertColor;
attribute vec2 vertTexCoord;

varying vec4 fragColor;
varying vec2 fragTexCoord;

uniform mat4 vertTransform; 

void main()
{
    gl_Position = vertTransform * vec4(vertPosition, 1.0);
    fragColor = vertColor;
    fragTexCoord = vertTexCoord;
}";

            string fragShader =
@"precision mediump float;

varying vec4 fragColor;
varying vec2 fragTexCoord;

uniform sampler2D fragTexture;

void main()
{
    gl_FragColor = texture2D(fragTexture, fragTexCoord) * fragColor;
}";

            uint vertexShader = GLUtils.CompileShader(vertShader, GL_VERTEX_SHADER);
            uint fragmentShader = GLUtils.CompileShader(fragShader, GL_FRAGMENT_SHADER);

            _program = glCreateProgram();
            if (_program == 0)
                throw new InvalidOperationException("Failed to create program.");

            glAttachShader(_program, vertexShader);
            glAttachShader(_program, fragmentShader);

            glBindAttribLocation(_program, 0, "vertPosition");
            glBindAttribLocation(_program, 1, "vertColor");
            glBindAttribLocation(_program, 2, "vertTexCoord");
            GLUtils.LinkProgram(_program);

            _vertTransformLocation = glGetUniformLocation(_program, "vertTransform");
            _fragTextureLocation = glGetUniformLocation(_program, "fragTexture");

            glEnable(GL_BLEND);
            glBlendFuncSeparate(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA, GL_ONE, GL_ZERO);
            glClearColor(1.0f, 0.0f, 1.0f, 1.0f);
        }

        public void BeginDraw()
        {
            glClear(GL_COLOR_BUFFER_BIT);
        }

        public void EndDraw()
        {
            Flush();
        }

        public void Shutdown()
        {
        }

        private void Flush()
        {
            if (_vertCount <= 0)
                return;

            int[] viewport = new int[4];

            fixed (int* viewportPtr = viewport)
            {
                glGetIntegerv(GL_VIEWPORT, viewportPtr);
            }

            float m11 = 2f / viewport[2];
            float m22 = -2f / viewport[3];

            float[] transform = new float[]
            {
                m11, 0.0f, 0.0f, 0.0f,
                0.0f, m22, 0.0f, 0.0f,
                0.0f, 0.0f, 1.0f, 0.0f,
                -1.0f, 1.0f, 0.0f, 1.0f,
            };

            glViewport(0, 0, _game.WindowWidth, _game.WindowHeight);
            glClear(GL_COLOR_BUFFER_BIT);

            glUseProgram(_program);

            fixed (void* vertPositionsPtr = _vertPositions)
            {
                glVertexAttribPointer(0, 3, GL_FLOAT, false, 0, vertPositionsPtr);
            }

            glEnableVertexAttribArray(0);

            fixed (void* vertColorsPtr = _vertColors)
            {
                glVertexAttribPointer(1, 4, GL_FLOAT, false, 0, vertColorsPtr);
            }

            glEnableVertexAttribArray(1);

            fixed (void* vertTexCoordsPtr = _vertTexCoords)
            {
                glVertexAttribPointer(2, 2, GL_FLOAT, false, 0, vertTexCoordsPtr);
            }

            glEnableVertexAttribArray(2);

            fixed (float* transformPtr = transform)
            {
                glUniformMatrix4fv(_vertTransformLocation, 1, false, transformPtr);
            }

            glActiveTexture(GL_TEXTURE0);
            glBindTexture(GL_TEXTURE_2D, _texture.Handle);

            glUniform1i(_fragTextureLocation, 0);

            fixed (void* indicesPtr = _indices)
            {
                glDrawElements(GL_TRIANGLES, _indexCount, GL_UNSIGNED_SHORT, indicesPtr);
            }

            _vertCount = 0;
            _indexCount = 0;
        }

        public void DrawSprite(Texture texture, Vector2 pos, int srcX, int srcY, int srcWidth, int srcHeight, Vector4 color)
        {
            if (texture.Handle != _texture.Handle)
                Flush();

            _texture = texture;

            float halfWidth = srcWidth / 2.0f;
            float halfHeight = srcHeight / 2.0f;

            _vertPositions[_vertCount] = new Vector3(pos.X - halfWidth, pos.Y - halfHeight, 0.0f);
            _vertPositions[_vertCount + 1] = new Vector3(pos.X + halfWidth, pos.Y - halfHeight, 0.0f);
            _vertPositions[_vertCount + 2] = new Vector3(pos.X + halfWidth, pos.Y + halfHeight, 0.0f);
            _vertPositions[_vertCount + 3] = new Vector3(pos.X - halfWidth, pos.Y + halfHeight, 0.0f);

            _vertColors[_vertCount] = color;
            _vertColors[_vertCount + 1] = color;
            _vertColors[_vertCount + 2] = color;
            _vertColors[_vertCount + 3] = color;

            _vertTexCoords[_vertCount] = new Vector2(srcX / (float)texture.Width, srcY / (float)texture.Height);
            _vertTexCoords[_vertCount + 1] = new Vector2((srcX + srcWidth) / (float)texture.Width, srcY / (float)texture.Height);
            _vertTexCoords[_vertCount + 2] = new Vector2((srcX + srcWidth) / (float)texture.Width, (srcY + srcHeight) / (float)texture.Height);
            _vertTexCoords[_vertCount + 3] = new Vector2(srcX / (float)texture.Width, (srcY + srcHeight) / (float)texture.Height);

            _indices[_indexCount] = (ushort)_vertCount;
            _indices[_indexCount + 1] = (ushort)(_vertCount + 1);
            _indices[_indexCount + 2] = (ushort)(_vertCount + 3);
            _indices[_indexCount + 3] = (ushort)(_vertCount + 1);
            _indices[_indexCount + 4] = (ushort)(_vertCount + 2);
            _indices[_indexCount + 5] = (ushort)(_vertCount + 3);

            _vertCount += 4;
            _indexCount += 6;
        }
    }
}
