using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investments.VectorsFileReader
{
    public class MonteCarlo
    {
        private static Dictionary<int, double> minimalProfitToAsk = new Dictionary<int, double>()
        {
            {1, 43.2796104045151 },
            {2, 42.8329931636058 },
            {3, 42.3828592515082 },
            {4, 41.9005887479511 },
            {5, 41.3854215545832 },
            {6, 40.8365418155675 },
            {7, 40.2530757585218 },
            {8, 39.6042937578739 },
            {9, 38.8881391384021 },
            {10, 38.1023288602609 },
            {11, 37.244341339512 },
            {12, 36.3114029055107 },
            {13, 35.2362230141439 },
            {14, 34.011833200481 },
            {15, 32.5966256332095 },
            {16, 30.857946054001 },
            {17, 28.7711637417476 },
            {18, 26.1384016927083 },
            {19, 22.7433333333333 },
            {20, -30 }

        };

        public static bool ShouldAsk(int[] profits, int stoppingDecision)
        {
            var ask = profits[stoppingDecision] >= minimalProfitToAsk[stoppingDecision + 1];
            return ask;
        }
        
    }
}
