using System;
using System.Collections.Generic;
using System.Linq;
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

        public int SelectedCount => _blocks.Count(x => x.IsSelected);

        public Board(RNG random)
        {
            _blocks = new Block[Width * Height];

            for (int i = 0; i < _blocks.Length; i++)
            {
                _blocks[i] = new Block(random);
            }

            _searchedBlocks = new bool[_blocks.Length];
        }

        public void LeftClick(int x, int y)
        {
            if (x < 0 || x >= Width ||
                y < 0 || y >= Height)
            {
                return;
            }

            Block block = _blocks[y * Width + x];

            if (block.IsSelected && SelectedCount > 1)
            {
                HideSelectedBlocks();
                MoveBlocksDown();
            }
            else
            {
                DeselectAllBlocks();

                if (!block.IsHidden)
                {
                    ClearSearchedBlocks();
                    SelectBlocks(x, y, block);
                }
            }
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

        private void MoveBlocksDown()
        {
            bool done = false;

            while (!done)
            {
                done = true;

                for (int y = Height - 2; y >= 0; y--)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        var block = this[x, y];

                        if (block.IsHidden)
                            continue;
                        
                        var blockBelow = this[x, y + 1];

                        if (blockBelow.IsHidden)
                        {
                            blockBelow.Copy(block);
                            block.Hide();
                            done = false;
                        }
                    }
                }
            }
        }
    }
}
