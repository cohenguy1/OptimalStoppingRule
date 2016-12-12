using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investments.VectorsFileReader
{
    public class Optimal
    {
        private static Dictionary<int, double> minimalProfitToAsk = new Dictionary<int, double>()
        {
            {1, 28.0184273399789 },
            {2, 27.8787253695423 },
            {3, 27.7268754016764 },
            {4, 27.5618210887787 },
            {5, 27.3824142269334 },
            {6, 27.1874067684058 },
            {7, 26.9743258731884 },
            {8, 26.7321884922596 },
            {9, 26.4570323775677 },
            {10, 26.1443549745087 },
            {11, 25.7815940876573 },
            {12, 25.3548165737145 },
            {13, 24.8473372850176 },
            {14, 24.22846010368 },
            {15, 23.4605751296 },
            {16, 22.47444096 },
            {17, 21.131168 },
            {18, 19.0336 },
            {19, 15.07 },
            {20, 0 }
        };

        public static bool ShouldAsk(int[] profits, int stoppingDecision)
        {
            var ask = profits[stoppingDecision] >= minimalProfitToAsk[stoppingDecision + 1];
            return ask;
        }
    }
}
