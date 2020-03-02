using System;
using System.Collections.Generic;
using System.Text;

namespace SameGame.Logic
{
    public class Board
    {
        public int Width { get; } = 32;
        public int Height { get; } = 20;

        private Block[] _blocks;

        public Block this[int x, int y] => _blocks[y * Width + x];

        public Board(Random random)
        {
            _blocks = new Block[Width * Height];

            for (int i = 0; i < _blocks.Length; i++)
            {
                _blocks[i] = new Block(random);
            }
        }
    }
}
