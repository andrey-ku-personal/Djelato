using System;
using System.Collections.Generic;
using System.Text;

namespace Djelato.Common.Shared
{
    public static class RandomGenerator
    {
        private static Random _rnd;
        static RandomGenerator()
        {
            _rnd = new Random();
        }

        private static readonly object syncLock = new object();
        public static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            {
                var result = _rnd.Next(min, max);
                return result;
            }
        }
    }
}
