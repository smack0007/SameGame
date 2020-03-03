using System.Numerics;
using SameGame.Logic;

namespace SameGame.Rendering
{
    public class BoardRenderer
    {
        public const int BlockWidth = 32;
        public const int BlockHeight = 32;

        private readonly Graphics _graphics;

        private Texture _blockTexture;

        public BoardRenderer(Graphics graphics)
        {
            _graphics = graphics;
        }

        public void Initialize()
        {
            _blockTexture = _graphics.LoadTexture(@"assets\Block.png");
        }

        public void Shutdown()
        {
        }

        private Vector4 CalculateBlockColor(BlockColor color)
        {
            return new Vector4(
                color == BlockColor.Red || color == BlockColor.Yellow ? 1.0f : 0.0f,
                color == BlockColor.Green || color == BlockColor.Yellow ? 1.0f : 0.0f,
                color == BlockColor.Blue ? 1.0f : 0.0f,
                1.0f
            );
        }

        public void Render(Board board)
        {
            var blockOrigin = new Vector2(BlockWidth / 2.0f, BlockHeight / 2.0f);

            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    Block block = board[x, y];

                    var srcX = 0;

                    if (block.Flags.HasFlag(BlockFlag.X2))
                    {
                        srcX = BlockWidth;
                    }
                    else if (block.Flags.HasFlag(BlockFlag.X3))
                    {
                        srcX = BlockWidth * 2;
                    }
                    else if (block.Flags.HasFlag(BlockFlag.X5))
                    {
                        srcX = BlockWidth * 3;
                    }

                    var position = new Vector2(x * BlockWidth, y * BlockHeight) + blockOrigin;

                    _graphics.DrawSprite(
                        _blockTexture,
                        position,
                        srcX,
                        0,
                        BlockWidth,
                        BlockHeight,
                        CalculateBlockColor(block.Color));

                    if (block.Flags.HasFlag(BlockFlag.Selected))
                    {
                        _graphics.DrawSprite(
                            _blockTexture,
                            position,
                            BlockWidth * 4,
                            0,
                            BlockWidth,
                            BlockHeight,
                            new Vector4(0, 0, 0, 0.5f));
                    }
                }
            }
        }
    }
}
