using System.Linq;

namespace SameGame.Logic
{
    public class Board
    {
        public const float TimeBetweenFills = 5000.0f;

        public int Width { get; } = 32;
        public int Height { get; } = 20;

        private readonly RNG _random;

        private Block[] _blocks;
        private bool[] _searchedBlocks;

        private float _timeToNextFill = TimeBetweenFills;

        public Block this[int x, int y] => _blocks[y * Width + x];

        public int SelectedCount => _blocks.Count(x => x.IsSelected);

        public Board(RNG random)
        {
            _random = random;

            _blocks = new Block[Width * Height];

            for (int i = 0; i < _blocks.Length; i++)
            {
                _blocks[i] = new Block(_random);
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

        public void Update(float elapsed)
        {
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
    }
}
