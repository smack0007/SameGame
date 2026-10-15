using System;
using System.Numerics;

namespace SameGame.Gameplay;

public enum BlockColor
{
    Red,
    Green,
    Blue,
    Yellow
}

[Flags]
public enum BlockFlag
{
    None = 0,
    Hidden = 1,
    X2 = 2,
    X3 = 4,
    X5 = 8,
    Selected = 16
}

public class Block
{
    public const float FallRate = 0.05f;

    private Vector2 _boardOffset;

    public BlockColor Color { get; private set; }

    public BlockFlag Flags { get; private set; }

    public bool IsHidden => Flags.HasFlag(BlockFlag.Hidden);

    public bool IsSelected => Flags.HasFlag(BlockFlag.Selected);

    public bool IsX2 => Flags.HasFlag(BlockFlag.X2);

    public bool IsX3 => Flags.HasFlag(BlockFlag.X3);

    public bool IsX5 => Flags.HasFlag(BlockFlag.X5);

    public bool IsFalling => _boardOffset.Y > 0;

    public float BoardOffsetX => _boardOffset.X;

    public float BoardOffsetY => _boardOffset.Y;

    public Block(RNG random)
    {
        Fill(random, 0.0f);
    }

    public void Fill(RNG random, float boardOffsetY)
    {
        Color = (BlockColor)random.Next(0, 4);

        Flags = BlockFlag.None;

        var modifierDistribution = random.Next(1, 101);
        if (modifierDistribution % 2 == 0)
        {
            Flags = BlockFlag.X2;
        }
        else if (modifierDistribution % 3 == 0)
        {
            Flags = BlockFlag.X3;
        }
        else if (modifierDistribution % 5 == 0)
        {
            Flags = BlockFlag.X5;
        }

        if (random.Next(1, 11) % 3 != 0)
            Flags = BlockFlag.None;

        _boardOffset = new Vector2(0, boardOffsetY);
    }

    public void Select()
    {
        Flags |= BlockFlag.Selected;
    }

    public void Deselect()
    {
        Flags &= ~BlockFlag.Selected;
    }

    public void Hide()
    {
        Flags = BlockFlag.Hidden;
    }

    public void Fall(float elapsed, float maxBoardOffsetY)
    {
        _boardOffset.Y += (FallRate * elapsed);

        if (_boardOffset.Y > maxBoardOffsetY)
            _boardOffset.Y = maxBoardOffsetY;
    }

    public void Copy(Block other)
    {
        Color = other.Color;
        Flags = other.Flags;
        _boardOffset = new Vector2();
    }
}

