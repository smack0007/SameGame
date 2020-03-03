using System;

namespace SameGame.Logic
{
    public class Block
    {
        public BlockColor Color { get; }

        public BlockFlag Flags { get; private set; }

        public bool IsSelected => Flags.HasFlag(BlockFlag.Selected);

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
    }
}
