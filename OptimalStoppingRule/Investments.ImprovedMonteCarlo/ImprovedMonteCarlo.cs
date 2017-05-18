using GamesCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Investments.ImprovedMonteCarlo
{
    public class ImprovedMonteCarlo
    {
        public static Dictionary<int, double> ChangeProbabilities = new Dictionary<int, double>();

        public static int[] ChangeProbabilitiesArray = new int[Constants.InvestmentsNumOfChanges];

        public const double alpha = 0.347;

        public const double delta = 0.01;

        public static double[] Thresholds = new double[Constants.TotalInvestmentsTurns];

        public static void Main(string[] args)
        {
            InitializeChangeProbabilities();

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

                Console.WriteLine("Threshold " + turnIndex + ": " + Thresholds[turnIndex]);
            }

            Console.ReadLine();
        }

        public static double FindThreshold(int stoppingDecision)
        {
            Random random = new Random();

            double minThreshold = Thresholds[stoppingDecision + 1];
            double maxThreshold = ChangeProbabilitiesArray.Max();
            var currentThreshold = (minThreshold + maxThreshold) / 2.0;
            var monteCarloThreshold = 0.0;

            while (Math.Abs(currentThreshold - monteCarloThreshold) > delta)
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
