using GamesCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Restaurant.MonteCarloDecider
{
    public class ImprovedMonteCarlo
    {
        public const double alpha = 0.347;

        public const double delta = 0.01;

        public const string MonteCarloFile = "MonteCarloProbs.txt";

        public static double[] Thresholds = new double[Constants.TotalRestaurantPositions];

        public static void Main(string[] args)
        {

            for (int turnIndex = Constants.TotalRestaurantPositions - 1; turnIndex >= 0; turnIndex--)
            {
                switch (turnIndex)
                {
                    case Constants.TotalRestaurantPositions - 1:
                        Thresholds[Constants.TotalRestaurantPositions - 1] = RestaurantProbabilities.AcceptedProbabilities.Max(keyVal => keyVal.Key);
                        break;
                    case Constants.TotalRestaurantPositions - 2:
                        Thresholds[Constants.TotalRestaurantPositions - 2] = GetExpectation();
                        break;
                    default:
                        Thresholds[turnIndex] = FindThreshold(turnIndex);
                        break;
                }

                Console.WriteLine("Threshold " + turnIndex + ": " + Thresholds[turnIndex]);
            }

            WriteMonteCarlo();

            Console.ReadLine();
        }

        public static void WriteMonteCarlo()
        {
            if (File.Exists(MonteCarloFile))
            {
                File.Delete(MonteCarloFile);
            }

            FileStream output = new FileStream(MonteCarloFile, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(output);


            sw.WriteLine("Dictionary for thresholds: ");
            for (int i = Constants.TotalRestaurantPositions; i > 0; i--)
            {
                sw.WriteLine("{" + i + ", " + Thresholds[i - 1] + " }, ");
            }

            sw.Close();
            output.Close();



        }

        public static double FindThreshold(int stoppingDecision)
        {
            Random random = new Random();

            double minThreshold = Thresholds[stoppingDecision + 1];
            double maxThreshold = RestaurantProbabilities.AcceptedProbabilities.Min(keyVal => keyVal.Key);
            var currentThreshold = (minThreshold + maxThreshold) / 2.0;
            var monteCarloThreshold = 0.0;

            while (Math.Abs(currentThreshold - monteCarloThreshold) > delta)
            {
                currentThreshold = (minThreshold + maxThreshold) / 2.0;

                var avgExponentialSmoothing = 0.0;
                for (int i = 0; i < Constants.MonteCarloSimulations; i++)
                {
                    double prevExponentialSmoothing = currentThreshold;
                    for (int turnIndex = stoppingDecision + 1; turnIndex < Constants.TotalRestaurantPositions; turnIndex++)
                    {
                        var randomChange = GetRandomAccepted(random);

                        var exponentialSmoothing = randomChange * alpha + prevExponentialSmoothing * (1 - alpha);

                        if (exponentialSmoothing < Thresholds[turnIndex] ||
                            (turnIndex == Constants.TotalRestaurantPositions - 1))
                        {
                            // better than threshold or last turn
                            avgExponentialSmoothing += exponentialSmoothing;
                            break;
                        }

                        prevExponentialSmoothing = exponentialSmoothing;
                    }
                }

                avgExponentialSmoothing /= Constants.MonteCarloSimulations;

                if (avgExponentialSmoothing < currentThreshold)
                {
                    minThreshold = currentThreshold;
                }
                else
                {
                    maxThreshold = currentThreshold;
                }

                monteCarloThreshold = avgExponentialSmoothing;
            }

            return currentThreshold;
        }

        public static double GetExpectation()
        {
            double expectation = 0;
            foreach (var pair in RestaurantProbabilities.AcceptedProbabilities)
            {
                expectation += pair.Key * pair.Value;
            }

            return expectation;
        }

        private static int GetRandomAccepted(Random randomSeed)
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
