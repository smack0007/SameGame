namespace SameGame
{
    public readonly struct Texture
    {
        public static readonly Texture None = new Texture(0, 0, 0);

        public uint Handle { get; }

        public int Width { get; }

        public int Height { get; }

        public Texture(uint handle, int width, int height)
        {
            Handle = handle;
            Width = width;
            Height = height;
        }
    }
}
