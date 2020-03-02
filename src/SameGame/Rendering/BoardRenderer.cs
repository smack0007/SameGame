using System.Numerics;
using SameGame.Logic;

namespace SameGame.Rendering
{
    public class BoardRenderer
    {
        private const int BlockWidth = 32;
        private const int BlockHeight = 32;

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
                    var srcX = 0;

                    if (board[x, y].Modifier == BlockModifier.X2)
                    {
                        srcX = BlockWidth;
                    }
                    else if (board[x, y].Modifier == BlockModifier.X3)
                    {
                        srcX = BlockWidth * 2;
                    }
                    else if (board[x, y].Modifier == BlockModifier.X5)
                    {
                        srcX = BlockWidth * 3;
                    }

                    _graphics.DrawSprite(
                        _blockTexture,
                        new Vector2(x * BlockWidth, y * BlockHeight) + blockOrigin,
                        srcX,
                        0,
                        BlockWidth,
                        BlockHeight,
                        CalculateBlockColor(board[x, y].Color));
                }
            }
        }
    }
}
