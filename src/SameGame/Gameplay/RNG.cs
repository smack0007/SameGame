using System;

namespace SameGame.Gameplay;

public class RNG
{
    private const int _a = 16807;
    private const int _m = 2147483647;
    private const int _q = 127773;
    private const int _r = 2836;

    private int _seed;

    public RNG(int seed)
    {
        if (seed <= 0 || seed == int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(seed));

        _seed = seed;
    }

    public double NextDouble()
    {
        int hi = _seed / _q;
        int lo = _seed % _q;

        _seed = (_a * lo) - (_r * hi);

        if (_seed <= 0)
            _seed = _seed + _m;

        return _seed * 1.0 / _m;
    }

    public int Next(int min, int max)
    {
        int range = max - min;
        return min + (int)(range * NextDouble());
    }
}
