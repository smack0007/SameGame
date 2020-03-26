using System;
using System.Numerics;

namespace SameGame.Logic
{
    public class Block
    {
        public const float FallRate = 0.05f;

        private Vector2 _boardOffset;

        public BlockColor Color { get; private set; }

        public BlockFlag Flags { get; private set; }

        public bool IsHidden => Flags.HasFlag(BlockFlag.Hidden);

        public bool IsSelected => Flags.HasFlag(BlockFlag.Selected);

        public bool IsFalling => _boardOffset.Y > 0;

        public float BoardOffsetX => _boardOffset.X;

        public float BoardOffsetY => _boardOffset.Y;

        public Block(RNG random)
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
        }

        public Block(BlockColor color, BlockFlag modifier)
        {
            Color = color;
            Flags = modifier;
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

        public void Fall(float elapsed)
        {
            _boardOffset.Y += (FallRate * elapsed);
        }

        public void Copy(Block other)
        {
            Color = other.Color;
            Flags = other.Flags;
            _boardOffset = new Vector2();
        }
    }
}
