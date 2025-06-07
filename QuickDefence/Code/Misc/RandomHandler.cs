using System;

namespace QuickDefence.Code.Misc
{
    internal static class RandomHandler
    {
        public static Random _random = new();

        public static double GenerateRandomDouble(double minimum, double maximum)
        {
            return _random.NextDouble() * (maximum - minimum) + minimum;
        }

        public static float GenerateRandomFloat(float minimum, float maximum)
        {
            return (float)_random.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}
