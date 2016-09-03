﻿using RestaurantCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantGameOptimalStopping
{
    class Program
    {
        public const int Samples = 10;

        public const int NumOfRanks = Constants.TotalCandidates;

        public static Dictionary<int, double> ChosenRankProbabilities = new Dictionary<int, double>()
        {
            /*10 modified*/
            {1, 0.25976076 },
            {2, 0.21542064 },
            {3, 0.16807679 },
            {4, 0.123697 },
            {5, 0.08855153 },
            {6, 0.06131535 },
            {7, 0.04052038 },
            {8, 0.02625731 },
            {9, 0.01375213 },
            {10, 0.00264811 },
            /* 15 modified
            {1, 0.20175 },
            {2, 0.169805556 },
            {3, 0.135 },
            {4, 0.110388889 },
            {5, 0.089388889 },
            {6, 0.071305556 },
            {7, 0.05675 },
            {8, 0.044888889 },
            {9, 0.035888889 },
            {10, 0.029361111 },
            {11, 0.021888889 },
            {12, 0.015777778 },
            {13, 0.010833333 },
            {14, 0.006055556 },
            {15, 0.000916667 },
            
            */
            /* 20 unmodified
            {1, 0.316205685},
            {2, 0.238760195},
            {3, 0.16912668 },
            {4, 0.110335625 },
            {5, 0.067916575 },
            {6, 0.039654305 },
            {7, 0.02307051 },
            {8, 0.012919295 },
            {9, 0.00718162 },
            {10, 0.00456783 },
            {11, 0.002691555 },
            {12, 0.00114881 },
            {13, 0.000978745 },
            {14, 0.000875835 },
            {15, 0.000689325 },
            {16, 0.000788975 },
            {17, 0.000698785 },
            {18, 0.000778985 },
            {19, 0.00083386 },
            {20, 0.000776805 }*/
        };

        public static int[] stoppingValues = new int[Samples];

        public static void Main(string[] args)
        {
            var expectation = GetExpectation(ChosenRankProbabilities);

            stoppingValues[Samples - 1] = NumOfRanks;

            for (int i = Samples - 2; i >= 0; i--)
            {
                var stoppingValue = GetProbabilityThatRankLowerOrEqualsThan(stoppingValues[i + 1]) * GetExpectationOrRankLowerOrEquals(stoppingValues[i + 1])
                                    + GetProbabilityThatRankGreater(stoppingValues[i + 1]) * stoppingValues[i + 1];
                stoppingValues[i] = stoppingValue > 1 ? (int)stoppingValue : 1;
            }

            for (int i = Samples - 1; i >= 0; i--)
            {
                Console.WriteLine((i + 1) + " Stopping Value: " + stoppingValues[i]);
            }

            Console.WriteLine("******");

            
            Console.ReadLine();
        }

        private static double GetProbabilityThatRankGreater(int rank)
        {
            double accumulatedProbability = 0;

            for (int i = rank + 1; i <= NumOfRanks; i++)
            {
                accumulatedProbability += ChosenRankProbabilities[i];
            }

            return accumulatedProbability;
        }

        private static double GetExpectationOrRankLowerOrEquals(int rank)
        {
            double expectation = 0;

            for (int i = 1; i <= rank; i++)
            {
                expectation += i * ChosenRankProbabilities[i];
            }

            return expectation;
        }

        private static double GetExpectation(Dictionary<int, double> chosenRankProbabilities)
        {
            double expectation = 0;

            for (int i = 1; i <= NumOfRanks; i++)
            {
                expectation += i * chosenRankProbabilities[i];
            }

            return expectation;
        }

        private static double GetProbabilityThatRankLowerOrEqualsThan(int rank)
        {
            double accumulatedProbability = 0;

            for (int i = 1; i <= rank; i++)
            {
                accumulatedProbability += ChosenRankProbabilities[i];
            }

            return accumulatedProbability;
        }
    }
}
