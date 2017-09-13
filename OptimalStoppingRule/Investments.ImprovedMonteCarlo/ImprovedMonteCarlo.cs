using GamesCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Investments.ImprovedMonteCarlo
{
    public class ImprovedMonteCarlo
    {
        public static string DifferencesFile = "Differences.txt";

        public static string ThresholdsFile = "Thresholds.txt";

        public static Dictionary<int, double> ChangeProbabilities = new Dictionary<int, double>();

        public static int[] ChangeProbabilitiesArray = new int[Constants.InvestmentsNumOfChanges];

        public const double alpha = 0.347;

        public const double delta = 0.01;

        public static double[] Thresholds = new double[Constants.TotalInvestmentsTurns];

        public static double[] AverageThresholds = new double[Constants.TotalInvestmentsTurns];

        public static double[][] DifferencesFromBaseline = new double[Constants.RepetitionsForDifferences][];

        public static double[] AverageDiffsFromBaseline = new double[Constants.TotalInvestmentsTurns];

        public static double[] StdDev = new double[Constants.TotalInvestmentsTurns];

        public static void Main(string[] args)
        {
            for (int i = 0; i < Constants.RepetitionsForDifferences; i++)
            {
                DifferencesFromBaseline[i] = new double[Constants.TotalInvestmentsTurns];
            }

            InitializeChangeProbabilities();

            for (int i = 0; i < Constants.RepetitionsForDifferences; i++)
            {
                FindAllThresholds();

                AggregateDifferencesFromBaseline(i);

                Console.WriteLine("Finished " + (i + 1) + "/" + Constants.RepetitionsForDifferences);
            }

            Console.WriteLine("Success! Press any key to exit");

            FileStream fs = new FileStream(DifferencesFile, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            for (int turnIndex = 0; turnIndex < Constants.TotalInvestmentsTurns - 2; turnIndex++)
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

            for (int turnIndex = 0; turnIndex < Constants.TotalInvestmentsTurns - 2; turnIndex++)
            {
                for (int i = 0; i < Constants.RepetitionsForDifferences; i++)
                {
                    var diff = DifferencesFromBaseline[i][turnIndex] - AverageDiffsFromBaseline[turnIndex];
                    StdDev[turnIndex] += diff*diff;
                }
                StdDev[turnIndex] /= (Constants.RepetitionsForDifferences - 1);
                StdDev[turnIndex] = Math.Sqrt(StdDev[turnIndex]);
                var standardError = StdDev[turnIndex] / Math.Sqrt(Constants.RepetitionsForDifferences);
                var absoluteStandardError = standardError / BaselineThresholds.Thresholds[turnIndex] * 100;
                sw.WriteLine(absoluteStandardError);
            }
            sw.Close();
            fs.Close();


            for (int turnIndex = 0; turnIndex < Constants.TotalInvestmentsTurns - 1; turnIndex++)
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
            for (int turnIndex = 0; turnIndex < Constants.TotalInvestmentsTurns - 1; turnIndex++)
            {
                sw.WriteLine(AverageThresholds[turnIndex]);
            }
            sw.Close();
            fs.Close();
        }

        private static void AggregateDifferencesFromBaseline(int index)
        {
            for (int turnIndex = 0; turnIndex < Constants.TotalInvestmentsTurns; turnIndex++)
            {
                DifferencesFromBaseline[index][turnIndex] += Math.Abs(Thresholds[turnIndex] - BaselineThresholds.Thresholds[turnIndex]);

                AverageThresholds[turnIndex] += Thresholds[turnIndex];
            }
        }

        private static void FindAllThresholds()
        {
            for (int turnIndex = Constants.TotalInvestmentsTurns - 1; turnIndex >= 0; turnIndex--)
            {
                switch (turnIndex)
                {
                    case Constants.TotalInvestmentsTurns - 1:
                        Thresholds[Constants.TotalInvestmentsTurns - 1] = ChangeProbabilitiesArray.Min();
                        break;
                    case Constants.TotalInvestmentsTurns - 2:
                        Thresholds[Constants.TotalInvestmentsTurns - 2] = GetExpectation();
                        break;
                    default:
                        Thresholds[turnIndex] = FindThreshold(turnIndex);
                        break;
                }

                //Console.WriteLine("Threshold " + turnIndex + ": " + Thresholds[turnIndex]);
            }
        }

        public static double FindThreshold(int stoppingDecision)
        {
            Random random = new Random();

            double minThreshold = Thresholds[stoppingDecision + 1];
            double maxThreshold = ChangeProbabilitiesArray.Max();
            var currentThreshold = (minThreshold + maxThreshold) / 2.0;
            var monteCarloThreshold = 0.0;

            while (Math.Abs(currentThreshold - monteCarloThreshold) > delta && (maxThreshold - minThreshold > delta))
            {
                currentThreshold = (minThreshold + maxThreshold) / 2.0;

                var avgExponentialSmoothing = 0.0;
                for (int i = 0; i < Constants.MonteCarloSimulations; i++)
                {
                    double prevExponentialSmoothing = currentThreshold;
                    for (int turnIndex = stoppingDecision + 1; turnIndex < Constants.TotalInvestmentsTurns; turnIndex++)
                    {
                        var randomChange = GetRandomChange(random);
                        
                        var exponentialSmoothing = randomChange * alpha + prevExponentialSmoothing * (1 - alpha);

                        if (exponentialSmoothing > Thresholds[turnIndex] ||
                            (turnIndex == Constants.TotalInvestmentsTurns - 1))
                        {
                            // better than threshold or last turn
                            avgExponentialSmoothing += exponentialSmoothing;
                            break;
                        }

                        prevExponentialSmoothing = exponentialSmoothing;
                    }
                }

                avgExponentialSmoothing /= Constants.MonteCarloSimulations;

                if (avgExponentialSmoothing > currentThreshold)
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
            foreach (var change in ChangeProbabilities)
            {
                expectation += change.Key * change.Value;
            }

            return expectation;
        }

        public static void InitializeChangeProbabilities()
        {
            FileStream fs = new FileStream("NasdaqChange.txt", FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            for (int i = 0; i < Constants.InvestmentsNumOfChanges; i++)
            {
                string line = sr.ReadLine();

                var change = int.Parse(line);

                ChangeProbabilitiesArray[i] = change;

                if (!ChangeProbabilities.ContainsKey(change))
                {
                    ChangeProbabilities.Add(change, 0);
                }

                ChangeProbabilities[change] += 1.0 / Constants.InvestmentsNumOfChanges;
            }
        }

        private static int GetRandomChange(Random random)
        {
            var changeIndex = random.Next(Constants.InvestmentsNumOfChanges);

            return ChangeProbabilitiesArray[changeIndex];
        }

    }
}
