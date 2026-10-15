using System.Linq;
using System.Numerics;

namespace SameGame.Gameplay;

public class Board
{
    public const float TimeBetweenFills = 5000.0f;

    public const int BlockWidth = 32;
    public const int BlockHeight = 32;

    public int Width { get; } = 32;
    public int Height { get; } = 20;

    private Graphics _graphics;
    private readonly RNG _random;

    private Block[] _blocks;
    private bool[] _searchedBlocks;

    private float _timeToNextFill = TimeBetweenFills;

    public Block this[int x, int y] => _blocks[y * Width + x];

    public int SelectedCount => _blocks.Count(x => x.IsSelected);

    private Texture _blockTexture;

    public Board(Graphics graphics, RNG random)
    {
        _graphics = graphics;

        _random = random;

        _blocks = new Block[Width * Height];

        for (int i = 0; i < _blocks.Length; i++)
        {
            _blocks[i] = new Block(_random);
        }

        _searchedBlocks = new bool[_blocks.Length];
    }

    private void DeselectAllBlocks()
    {
        for (int i = 0; i < _blocks.Length; i++)
            _blocks[i].Deselect();
    }

    private void ClearSearchedBlocks()
    {
        for (int i = 0; i < _searchedBlocks.Length; i++)
            _searchedBlocks[i] = false;
    }

    private void SelectBlocks(int x, int y, Block source)
    {
        if (_searchedBlocks[y * Width + x])
            return;

        _searchedBlocks[y * Width + x] = true;
        Block current = _blocks[y * Width + x];

        if (current.IsHidden || current.Color != source.Color)
            return;

        current.Select();

        if (x > 0)
            SelectBlocks(x - 1, y, source);

        if (x < Width - 1)
            SelectBlocks(x + 1, y, source);

        if (y > 0)
            SelectBlocks(x, y - 1, source);

        if (y < Height - 1)
            SelectBlocks(x, y + 1, source);
    }

    private void HideSelectedBlocks()
    {
        for (int i = 0; i < _blocks.Length; i++)
            if (_blocks[i].IsSelected)
                _blocks[i].Hide();
    }

    public void Initialize()
    {
        _blockTexture = _graphics.LoadTexture(@"assets/Block.png");
    }

    public void Shutdown()
    {
    }

    private void OnLeftClick(Point position)
    {
        if (position.X < 0 || position.X >= Width ||
            position.Y < 0 || position.Y >= Height)
        {
            return;
        }

        Block block = _blocks[position.Y * Width + position.X];

        if (block.IsSelected && SelectedCount > 1)
        {
            HideSelectedBlocks();
        }
        else
        {
            DeselectAllBlocks();

            if (!block.IsHidden)
            {
                ClearSearchedBlocks();
                SelectBlocks(position.X, position.Y, block);
            }
        }
    }

    public void Update(float elapsed, Mouse mouse)
    {
        if (mouse.IsLeftMouseButtonPressed)
        {
            OnLeftClick(mouse.Position);
        }

        for (int y = Height - 2; y >= 0; y--)
        {
            for (int x = 0; x < Width; x++)
            {
                var block = this[x, y];

                if (block.IsHidden)
                    continue;

                var blockBelow = this[x, y + 1];

                if (blockBelow.IsHidden || blockBelow.IsFalling)
                {
                    block.Fall(elapsed, 1.0f);

                    if (block.BoardOffsetY >= 1)
                    {
                        blockBelow.Copy(block);
                        block.Hide();
                    }
                }
                else if (block.BoardOffsetY <= 0.0f)
                {
                    block.Fall(elapsed, 0.0f);
                }
            }
        }

        _timeToNextFill -= elapsed;

        if (_timeToNextFill <= 0)
        {
            for (int x = 0; x < Width; x++)
            {
                var block = this[x, 0];

                if (!block.IsHidden)
                    continue;

                block.Fill(_random, -1.0f);
            }

            _timeToNextFill += TimeBetweenFills;
        }
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

    public void Draw()
    {
        var origin = new Vector2(BlockWidth / 2.0f, BlockHeight / 2.0f);

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Block block = this[x, y];

                if (block.IsHidden)
                    continue;

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

                var offset = new Vector2(block.BoardOffsetX * BlockWidth, block.BoardOffsetY * BlockHeight);
                var position = new Vector2(x * BlockWidth, y * BlockHeight) + origin + offset;

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

