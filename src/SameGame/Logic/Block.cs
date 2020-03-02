using System;

namespace SameGame.Logic
{
    public class Block
    {
        public BlockColor Color { get; }

        public BlockModifier Modifier { get; }

        public Block(Random random)
        {
            Color = (BlockColor)random.Next(0, 4);

            Modifier = BlockModifier.None;

            var modifierDistribution = random.Next(1, 101);
            if (modifierDistribution % 2 == 0)
            {
                Modifier = BlockModifier.X2;
            }
            else if (modifierDistribution % 3 == 0)
            {
                Modifier = BlockModifier.X3;
            }
            else if (modifierDistribution % 5 == 0)
            {
                Modifier = BlockModifier.X5;
            }

            if (random.Next(1, 11) % 3 != 0)
                Modifier = BlockModifier.None;
        }

        public Block(BlockColor color, BlockModifier modifier)
        {
            Color = color;
            Modifier = modifier;
        }
    }
}
