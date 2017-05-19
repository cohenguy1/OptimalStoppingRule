using System.Collections.Generic;

namespace Restaurant.VectorsFileReader
{
    public class Optimal
    {
        private static Dictionary<int, double> minimalRankForAsk = new Dictionary<int, double>()
        {
            {10, 10 },
            {9, 3.7040364 },
            {8, 2.76637865095412 },
            {7, 2.28512684759564 },
            {6, 1.98256340569754 },
            {5, 1.78929701579412 },
            {4, 1.63404537104578 },
            {3, 1.50933111933802 },
            {2, 1.4091476745556 },
            {1, 1.32866992264644 }

        };

        public static bool ShouldAsk(int[] accepted, int stoppingDecision)
        {
            var ask = accepted[stoppingDecision] <= minimalRankForAsk[stoppingDecision + 1];
            return ask;
        }
    }
}
