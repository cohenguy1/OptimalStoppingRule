using GamesCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Restaurant.MonteCarloDecider
{
    public class ImprovedMonteCarlo
    {
        public static string DifferencesFile = "Differences.txt";

        public static string ThresholdsFile = "Thresholds.txt";

        public const double alpha = 0.312;

        public const double delta = 0.01;

        public const string MonteCarloFile = "MonteCarloProbs.txt";

        public static double[] Thresholds = new double[Constants.TotalRestaurantPositions];

        public static double[] AverageThresholds = new double[Constants.TotalRestaurantPositions];

        public static double[][] DifferencesFromBaseline = new double[Constants.RepetitionsForDifferences][];

        public static double[] AverageDiffsFromBaseline = new double[Constants.TotalRestaurantPositions];

        public static double[] StdDev = new double[Constants.TotalRestaurantPositions];


        public static void Main(string[] args)
        {
            for (int i = 0; i < Constants.RepetitionsForDifferences; i++)
            {
                DifferencesFromBaseline[i] = new double[Constants.TotalRestaurantPositions];
            }

            for (int i = 0; i < Constants.RepetitionsForDifferences; i++)
            {
                FindAllThresholds();

                AggregateDifferencesFromBaseline(i);

                Console.WriteLine("Finished " + (i + 1) + "/" + Constants.RepetitionsForDifferences);
            }

            //WriteMonteCarlo();

            FileStream fs = new FileStream(DifferencesFile, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            for (int turnIndex = 0; turnIndex < Constants.TotalRestaurantPositions - 2; turnIndex++)
            {
                for (int i = 0; i < Constants.RepetitionsForDifferences; i++)
                {
                    AverageDiffsFromBaseline[turnIndex] += DifferencesFromBaseline[i][turnIndex];
                }
                AverageDiffsFromBaseline[turnIndex] /= Constants.RepetitionsForDifferences;

                var absoluteError = AverageDiffsFromBaseline[turnIndex] / BaselineThresholds.Thresholds[turnIndex] * 100;

                sw.WriteLine((turnIndex + 1) + "\t" + absoluteError);
            }

            sw.WriteLine();
            sw.WriteLine("Standard Deviation:");

            for (int turnIndex = 0; turnIndex < Constants.TotalRestaurantPositions - 2; turnIndex++)
            {
                for (int i = 0; i < Constants.RepetitionsForDifferences; i++)
                {
                    var diff = DifferencesFromBaseline[i][turnIndex] - AverageDiffsFromBaseline[turnIndex];
                    StdDev[turnIndex] += diff * diff;
                }
                StdDev[turnIndex] /= (Constants.RepetitionsForDifferences - 1);
                StdDev[turnIndex] = Math.Sqrt(StdDev[turnIndex]);
                var standardError = StdDev[turnIndex] / Math.Sqrt(Constants.RepetitionsForDifferences);
                var absoluteStandardError = standardError / BaselineThresholds.Thresholds[turnIndex] * 100;
                sw.WriteLine(absoluteStandardError);
            }

            sw.Close();
            fs.Close();

            for (int turnIndex = 0; turnIndex < Constants.TotalRestaurantPositions - 1; turnIndex++)
            {
                AverageThresholds[turnIndex] /= Constants.RepetitionsForDifferences;
            }
            WriteThresholds(AverageThresholds);


            Console.ReadLine();
        }

        private static void WriteThresholds(double[] averageThresholds)
        {
            FileStream fs = new FileStream(ThresholdsFile, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            for (int turnIndex = 0; turnIndex < Constants.TotalRestaurantPositions - 1; turnIndex++)
            {
                sw.WriteLine(AverageThresholds[turnIndex]);
            }
            sw.Close();
            fs.Close();
        }

        private static void AggregateDifferencesFromBaseline(int index)
        {
            for (int turnIndex = 0; turnIndex < Constants.TotalRestaurantPositions; turnIndex++)
            {
                DifferencesFromBaseline[index][turnIndex] += Math.Abs(Thresholds[turnIndex] - BaselineThresholds.Thresholds[turnIndex]);

                AverageThresholds[turnIndex] += Thresholds[turnIndex];
            }
        }

        private static void FindAllThresholds()
        {
            for (int turnIndex = Constants.TotalRestaurantPositions - 1; turnIndex >= 0; turnIndex--)
            {
                Random random = new Random();

                switch (turnIndex)
                {
                    case Constants.TotalRestaurantPositions - 1:
                        Thresholds[Constants.TotalRestaurantPositions - 1] = RestaurantProbabilities.AcceptedProbabilities.Max(keyVal => keyVal.Key);
                        break;
                    case Constants.TotalRestaurantPositions - 2:
                        Thresholds[Constants.TotalRestaurantPositions - 2] = RestaurantProbabilities.GetExpectation();
                        break;
                    default:
                        Thresholds[turnIndex] = FindThreshold(turnIndex, random);
                        break;
                }

                //Console.WriteLine("Threshold " + turnIndex + ": " + Thresholds[turnIndex]);
            }
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

        public static double FindThreshold(int stoppingDecision, Random random)
        {
            double minThreshold = Thresholds[stoppingDecision + 1];
            double maxThreshold = RestaurantProbabilities.AcceptedProbabilities.Min(keyVal => keyVal.Key);
            var currentThreshold = (minThreshold + maxThreshold) / 2.0;
            var monteCarloThreshold = 0.0;

            while (Math.Abs(currentThreshold - monteCarloThreshold) > delta && (Math.Abs(maxThreshold - minThreshold) > delta))
            {
                currentThreshold = (minThreshold + maxThreshold) / 2.0;

                var avgExponentialSmoothing = 0.0;
                for (long i = 0; i < Constants.MonteCarloSimulations; i++)
                {
                    double prevExponentialSmoothing = currentThreshold;
                    for (int turnIndex = stoppingDecision + 1; turnIndex < Constants.TotalRestaurantPositions; turnIndex++)
                    {
                        var randomChange = RestaurantProbabilities.GetRandomAccepted(random);

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
    }
}
