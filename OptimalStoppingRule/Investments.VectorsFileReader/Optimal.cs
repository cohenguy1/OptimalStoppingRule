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
            {1, 76.0517480409671 },
            {2, 75.396144846918 },
            {3, 74.6900843671777 },
            {4, 73.9270996720923 },
            {5, 73.0917149694442 },
            {6, 72.1710091574752 },
            {7, 71.1492694342316 },
            {8, 70.0103376589944 },
            {9, 68.7200806730997 },
            {10, 67.2424007766535 },
            {11, 65.531738351336 },
            {12, 63.5485329666698 },
            {13, 61.2226767322872 },
            {14, 58.4364494724135 },
            {15, 54.9869567732939 },
            {16, 50.5092346548149 },
            {17, 44.4899567407408 },
            {18, 36.1153222222222 },
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
