using System;
using System.Linq;
using System.Numerics;

namespace SameGame.Gameplay;

public class Board
{
    public const float TimeBetweenFills = 5000.0f;

    public const int BlockWidthInPixels = 32;
    public const int BlockHeightInPixels = 32;

    public int WidthInBlocks { get; } = 32;
    public int HeightInBlocks { get; } = 20;

    private Graphics _graphics;
    private readonly RNG _random;

    private Block[] _blocks;
    private bool[] _searchedBlocks;
    private Rectangle _rectangle;

    private Texture _blockTexture;


    private float _timeToNextFill = TimeBetweenFills;

    public Block this[int x, int y] => _blocks[y * WidthInBlocks + x];

    public int SelectedCount => _blocks.Count(x => x.IsSelected);

    public int Score { get; private set; }

    public Point Offset
    {
        get { return _rectangle.TopLeft(); }
        set { _rectangle = new Rectangle(value.X, value.Y, _rectangle.Width, _rectangle.Height); }
    }

    public Board(Graphics graphics, RNG random, Point offset)
    {
        _graphics = graphics;

        _random = random;

        _blocks = new Block[WidthInBlocks * HeightInBlocks];

        for (int i = 0; i < _blocks.Length; i++)
        {
            _blocks[i] = new Block(_random);
        }

        _searchedBlocks = new bool[_blocks.Length];

        _rectangle = new Rectangle(offset.X, offset.Y, WidthInBlocks * BlockWidthInPixels, HeightInBlocks * BlockHeightInPixels);
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
        if (_searchedBlocks[y * WidthInBlocks + x])
            return;

        _searchedBlocks[y * WidthInBlocks + x] = true;
        Block current = _blocks[y * WidthInBlocks + x];

        if (current.IsHidden || current.Color != source.Color)
            return;

        current.Select();

        if (x > 0)
            SelectBlocks(x - 1, y, source);

        if (x < WidthInBlocks - 1)
            SelectBlocks(x + 1, y, source);

        if (y > 0)
            SelectBlocks(x, y - 1, source);

        if (y < HeightInBlocks - 1)
            SelectBlocks(x, y + 1, source);
    }

    private void HideSelectedBlocks()
    {
        int selectedBlockScore = this.SelectedCount;

        for (int i = 0; i < _blocks.Length; i += 1)
        {
            if (_blocks[i].IsSelected)
            {
                if (_blocks[i].IsX2)
                {
                    selectedBlockScore *= 2;
                }
                else if (_blocks[i].IsX3)
                {
                    selectedBlockScore *= 3;
                }
                else if (_blocks[i].IsX5)
                {
                    selectedBlockScore *= 5;
                }

                _blocks[i].Hide();
            }
        }

        Score += selectedBlockScore;
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
        if (position.X < 0 || position.X >= WidthInBlocks ||
            position.Y < 0 || position.Y >= HeightInBlocks)
        {
            return;
        }

        Block block = _blocks[position.Y * WidthInBlocks + position.X];

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
        if (mouse.IsLeftMouseButtonPressed && _rectangle.Intersects(mouse.Position))
        {
            OnLeftClick(new Point(
                (mouse.Position.X - _rectangle.X) / BlockWidthInPixels,
                (mouse.Position.Y - _rectangle.Y) / BlockHeightInPixels
            ));
        }

        for (int y = HeightInBlocks - 2; y >= 0; y--)
        {
            for (int x = 0; x < WidthInBlocks; x++)
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
            for (int x = 0; x < WidthInBlocks; x++)
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
        var origin = new Vector2(BlockWidthInPixels / 2.0f, BlockHeightInPixels / 2.0f);

        for (int y = 0; y < HeightInBlocks; y++)
        {
            for (int x = 0; x < WidthInBlocks; x++)
            {
                Block block = this[x, y];

                if (block.IsHidden)
                    continue;

                var srcX = 0;

                if (block.Flags.HasFlag(BlockFlag.X2))
                {
                    srcX = BlockWidthInPixels;
                }
                else if (block.Flags.HasFlag(BlockFlag.X3))
                {
                    srcX = BlockWidthInPixels * 2;
                }
                else if (block.Flags.HasFlag(BlockFlag.X5))
                {
                    srcX = BlockWidthInPixels * 3;
                }

                var offset = new Vector2(block.BoardOffsetX * BlockWidthInPixels, block.BoardOffsetY * BlockHeightInPixels);
                var position = new Vector2(x * BlockWidthInPixels + _rectangle.X, y * BlockHeightInPixels + _rectangle.Y) + origin + offset;

                _graphics.DrawSprite(
                    _blockTexture,
                    position,
                    srcX,
                    0,
                    BlockWidthInPixels,
                    BlockHeightInPixels,
                    CalculateBlockColor(block.Color));

                if (block.Flags.HasFlag(BlockFlag.Selected))
                {
                    _graphics.DrawSprite(
                        _blockTexture,
                        position,
                        BlockWidthInPixels * 4,
                        0,
                        BlockWidthInPixels,
                        BlockHeightInPixels,
                        new Vector4(0, 0, 0, 0.5f));
                }
            }
        }
    }
}

