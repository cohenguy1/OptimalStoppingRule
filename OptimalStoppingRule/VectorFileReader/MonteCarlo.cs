﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.VectorsFileReader
{
    public class MonteCarlo
    {
        private static Dictionary<int, double> minimalRankToAsk = new Dictionary<int, double>()
        {
            {10, 10 },
            {9, 3.7040364 },
            {8, 3.4505329875 },
            {7, 3.29737467578125 },
            {6, 3.15378875854492 },
            {5, 3.05282991048813 },
            {4, 2.956603508434 },
            {3, 2.89545964879543 },
            {2, 2.83622653477058 },
            {1, 2.778844455559 },

        };

        public static bool ShouldAsk(int[] accepted, int stoppingDecision)
        {
            var ask = accepted[stoppingDecision] <= minimalRankToAsk[stoppingDecision + 1];
            return ask;
        }
        
    }
}
