using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorFileReader
{
    public class Optimal
    {
        private static Dictionary<int, int> minimalRankForAsk = new Dictionary<int, int>()
        {
            {10, 10 },
            {9, 3 },
            {8, 1 },
            {7, 1 },
            {6, 1 },
            {5, 1 },
            {4, 1 },
            {3, 1 },
            {2, 1 },
            {1, 1 }
        };

        public static bool ShouldAsk(int[] accepted, int stoppingDecision)
        {
            var ask = accepted[stoppingDecision] <= minimalRankForAsk[stoppingDecision + 1];
            return ask;
        }
    }
}
