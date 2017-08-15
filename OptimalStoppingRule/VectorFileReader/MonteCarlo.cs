using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.VectorsFileReader
{
    public class MonteCarlo
    {
        public const double Alpha = 0.312;

        private static Dictionary<int, double> minimalRankToAsk = new Dictionary<int, double>()
        {
            {10, 10 },
            {9, 3.7040364 },
            {8, 3.49278355625 },
            {7, 3.33698458398437 },
            {6, 3.19092304748535 },
            {5, 3.12245670225143 },
            {4, 3.05612993030608 },
            {3, 2.99187586998401 },
            {2, 2.92962974904701 },
            {1, 2.86932881938929 },

        };

        public static bool ShouldAsk(int[] accepted, int stoppingDecision)
        {
            double exponentialSmoothing = accepted[0];
            for (int i = 1; i <= stoppingDecision; i++)
            {
                exponentialSmoothing = Alpha * accepted[i] + (1 - Alpha) * exponentialSmoothing;
            }

            var ask = exponentialSmoothing <= minimalRankToAsk[stoppingDecision + 1];
            return ask;
        }
        
    }
}
