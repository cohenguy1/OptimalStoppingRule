using GamesCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantGameOptimalStopping
{
    class RestaurantOptimalDecision
    {
        public const string OptimalFile = "OptimalDecision.txt";

        public static double[] StoppingValues = new double[Constants.TotalRestaurantPositions];

        public static void Main(string[] args)
        {
            var expectation = RestaurantProbabilities.GetExpectation();

            StoppingValues[Constants.TotalRestaurantPositions - 1] = Constants.RestaurantNumOfCandidates;

            double[] expectedRankTi = new double[Constants.TotalRestaurantPositions + 1];

            var Eiplus1 = expectation;
            for (int i = Constants.TotalRestaurantPositions - 1; i > 0; i--)
            {
                var Ti = Eiplus1;
                StoppingValues[i - 1] = Ti;

                var Ei = GetProbabilityThatRankGreater(Ti) * Eiplus1 +
                    GetExpectedRatingLowerThanOrEquals(Ti);
                Eiplus1 = Ei;
            }

            for (int i = Constants.TotalRestaurantPositions - 1; i >= 0; i--)
            {
                Console.WriteLine((i + 1) + " Stopping Value: " + StoppingValues[i]);
            }

            Console.WriteLine("******");

            WriteOptimal();

            Console.ReadLine();
        }

        public static void WriteOptimal()
        {
            if (File.Exists(OptimalFile))
            {
                File.Delete(OptimalFile);
            }

            FileStream output = new FileStream(OptimalFile, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(output);


            sw.WriteLine("Dictionary for Optimal: ");
            for (int i = Constants.TotalRestaurantPositions - 1; i >= 0; i--)
            {
                sw.WriteLine("{" + (i + 1) + ", " + StoppingValues[i] + " }, ");
            }

            sw.Close();
            output.Close();
        }

        private static double GetExpectedRatingLowerThanOrEquals(double rank)
        {
            double expectation = 0;

            for (int i = 1; i <= (int)Math.Floor(rank); i++)
            {
                expectation += RestaurantProbabilities.AcceptedProbabilities[i] * i;
            }

            return expectation;
        }

        private static double GetProbabilityThatRankGreater(double rank)
        {
            double accumulatedProbability = 0;

            for (int i = (int)Math.Ceiling(rank); i <= Constants.RestaurantNumOfCandidates; i++)
            {
                accumulatedProbability += RestaurantProbabilities.AcceptedProbabilities[i];
            }

            return accumulatedProbability;
        }

        private static double GetExpectation(Dictionary<int, double> chosenRankProbabilities)
        {
            double expectation = 0;

            for (int i = 1; i <= Constants.RestaurantNumOfCandidates; i++)
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
                accumulatedProbability += RestaurantProbabilities.AcceptedProbabilities[i];
            }

            return accumulatedProbability;
        }
    }
}
