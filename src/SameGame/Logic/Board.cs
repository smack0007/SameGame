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
        private bool[] _searchedBlocks;

        public Block this[int x, int y] => _blocks[y * Width + x];

        public Board(RNG random)
        {
            _blocks = new Block[Width * Height];

            for (int i = 0; i < _blocks.Length; i++)
            {
                _blocks[i] = new Block(random);
            }

            _searchedBlocks = new bool[_blocks.Length];
        }

        public void LeftClickBlock(int x, int y)
        {
            DeselectAllBlocks();
            Block source = _blocks[y * Width + x];
            ClearSearchedBlocks();
            SelectBlocks(x, y, source);
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

            if (current.Color != source.Color)
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
    }
}
