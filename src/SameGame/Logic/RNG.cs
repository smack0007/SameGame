using System;
using System.Collections.Generic;
using System.Text;

namespace SameGame.Logic
{
    public class RNG
    {
        private const int a = 16807;
        private const int m = 2147483647;
        private const int q = 127773;
        private const int r = 2836;
        
        private int _seed;

        public RNG(int seed)
        {
            if (seed <= 0 || seed == int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(seed));

            _seed = seed;
        }

        public double NextDouble()
        {
            int hi = _seed / q;
            int lo = _seed % q;

            _seed = (a * lo) - (r * hi);
            
            if (_seed <= 0)
                _seed = _seed + m;

            return (_seed * 1.0) / m;
        }

        public int Next(int min, int max)
        {
            int range = max - min;
            return min + (int)(range * NextDouble());
        }
    }
}
