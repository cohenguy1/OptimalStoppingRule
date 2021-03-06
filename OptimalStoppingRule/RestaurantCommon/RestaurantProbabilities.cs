﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesCommon
{
    public class RestaurantProbabilities
    {
        public static Dictionary<int, double> AcceptedProbabilities = new Dictionary<int, double>()
        {
            {1, 0.1966961 },
            {2, 0.174603 },
            {3, 0.1537626 },
            {4, 0.133509 },
            {5, 0.1130325 },
            {6, 0.0913821 },
            {7, 0.0690814 },
            {8, 0.0456541 },
            {9, 0.0222392 },
            {10, 4E-05 },
        };

        public static double GetExpectation()
        {
            double expectation = 0;
            foreach (var pair in AcceptedProbabilities)
            {
                expectation += pair.Key * pair.Value;
            }

            return expectation;
        }

        public static int GetRandomAccepted(Random randomSeed)
        {
            var random = randomSeed.NextDouble();

            var currentThreshold = 0d;

            foreach (var pair in RestaurantProbabilities.AcceptedProbabilities)
            {
                if ((random < pair.Value + currentThreshold) && (random > currentThreshold))
                {
                    return pair.Key;
                }

                currentThreshold += pair.Value;
            }

            return RestaurantProbabilities.AcceptedProbabilities.Max(keyVal => keyVal.Key);
        }
    }
}
