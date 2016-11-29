using RestaurantCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantGameOptimalStopping
{
    class RateDecisionForOptimal
    {
        public const int NumOfRanks = Constants.TotalCandidates;

        public static Dictionary<int, double> ChosenRankProbabilities = new Dictionary<int, double>()
        {
            /*{1, 0.1 },
            {2, 0.1 },
            {3, 0.1 },
            {4, 0.1 },
            {5, 0.1 },
            {6, 0.1 },
            {7, 0.1 },
            {8, 0.1 },
            {9, 0.1 },
            {10, 0.1 },*/

            /*10 modified*/
            {1, 0.1968508 },
            {2, 0.1749329 },
            {3, 0.153611 },
            {4, 0.1334523 },
            {5, 0.1131547 },
            {6, 0.0914123 },
            {7, 0.0688202 },
            {8, 0.0454561 },
            {9, 0.0222743 },
            {10, 3.54E-05 }, 
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

        public static int[] stoppingValues = new int[Constants.TotalPositions];

        public static void Main(string[] args)
        {
            var expectation = GetExpectation(ChosenRankProbabilities);

            stoppingValues[Constants.TotalPositions - 1] = NumOfRanks;

            double[] expectedRankTi = new double[Constants.TotalPositions + 1];

            for (int i = Constants.TotalPositions - 2; i >= 0; i--)
            {
                for (int j = 1; j <= stoppingValues[i + 1]; j++)
                {
                    expectedRankTi[j] = GetExpectedRatingLowerThanOrEquals(j)
                                    + GetProbabilityThatRankGreater(j) * stoppingValues[i + 1];
                }

                stoppingValues[i] = (int)expectedRankTi[1];
                for (int j = 1; j <= stoppingValues[i + 1]; j++)
                {
                    if (expectedRankTi[j] < stoppingValues[i])
                    {
                        stoppingValues[i] = (int)expectedRankTi[j];
                    }
                    if (expectedRankTi[j] < 1)
                    {
                        stoppingValues[i] = 1;
                    }
                }
            }

            for (int i = Constants.TotalPositions - 1; i >= 0; i--)
            {
                Console.WriteLine((i + 1) + " Stopping Value: " + stoppingValues[i]);
            }

            Console.WriteLine("******");


            Console.ReadLine();
        }

        private static double GetExpectedRatingLowerThanOrEquals(int rank)
        {
            double expectation = 0;

            for (int i = 1; i <= rank; i++)
            {
                expectation += ChosenRankProbabilities[i] * i;
            }

            return expectation;
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
