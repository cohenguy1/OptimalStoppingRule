using GamesCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalStoppingDecision
{
    class Program
    {
        public static Dictionary<double, double> ChangeProbabilities = new Dictionary<double, double>();

        public static double[] stoppingValues = new double[Constants.TotalInvestmentsTurns];

        public static void Main(string[] args)
        {
            InitializeChangeProbabilities();

            var expectation = GetExpectation(ChangeProbabilities);

            var minProb = ChangeProbabilities.Min(kv => kv.Key);
            var maxProb = ChangeProbabilities.Max(kv => kv.Key);

            // get worst probability
            stoppingValues[Constants.TotalInvestmentsTurns - 1] = minProb;

            Dictionary<double, double> expectedChangeTi = new Dictionary<double, double>();

            for (int i = Constants.TotalInvestmentsTurns - 2; i >= 0; i--)
            {
                for (double j = maxProb; j >= stoppingValues[i + 1]; j -= 0.01)
                {
                    if (!expectedChangeTi.ContainsKey(j))
                    {
                        expectedChangeTi.Add(j, 0);
                    }
                    
                    expectedChangeTi[j] = GetExpectedChangeGreaterThanOrEquals(j)
                                    + GetProbabilityThatChangeLess(j) * stoppingValues[i + 1];
                }

                stoppingValues[i] = expectedChangeTi.First().Value;
                for (double j = maxProb; j >= stoppingValues[i + 1]; j -= 0.01)
                {
                    if (expectedChangeTi[j] > stoppingValues[i])
                    {
                        stoppingValues[i] = expectedChangeTi[j];
                    }
                }
            }

            for (int i = Constants.TotalInvestmentsTurns - 1; i >= 0; i--)
            {
                Console.WriteLine((i + 1) + " Stopping Value: " + stoppingValues[i]);
            }

            Console.WriteLine("******");


            Console.ReadLine();
        }

        private static void InitializeChangeProbabilities()
        {
            FileStream fs = new FileStream("NasdaqChange.txt", FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            for (int i = 0; i < Constants.NumOfChanges; i++)
            {
                string line = sr.ReadLine();

                var change = double.Parse(line);

                change = Math.Round(change, 2);

                if (!ChangeProbabilities.ContainsKey(change))
                {
                    ChangeProbabilities.Add(change, 0);
                }

                ChangeProbabilities[change] += 1.0 / Constants.NumOfChanges;
            }
        }

        private static double GetExpectedChangeGreaterThanOrEquals(double specificChange)
        {
            double expectation = 0;

            foreach (var change in ChangeProbabilities)
            {
                if (change.Key >= specificChange)
                {
                    expectation += change.Key * change.Value;
                }
            }

            return expectation;
        }

        private static double GetProbabilityThatChangeLess(double specificChange)
        {
            double accumulatedProbability = 0;

            foreach (var change in ChangeProbabilities)
            {
                if (change.Key < specificChange)
                {
                    accumulatedProbability += change.Value;
                }
            }

            return accumulatedProbability;
        }

        private static double GetExpectationOfChangeGreaterOrEquals(double specificChange)
        {
            double expectation = 0;

            foreach (var change in ChangeProbabilities)
            {
                if (change.Key >= specificChange)
                {
                    expectation += change.Key * change.Value;
                }
            }

            return expectation;
        }

        private static double GetExpectation(Dictionary<double, double> changeProbabilities)
        {
            double expectation = 0;

            foreach (var change in changeProbabilities)
            {
                expectation += change.Key * change.Value;
            }

            return expectation;
        }

        private static double GetProbabilityThatChangeGreaterOrEqualsThan(double specificChange)
        {
            double accumulatedProbability = 0;

            foreach (var change in ChangeProbabilities)
            {
                if (change.Key >= specificChange)
                {
                    accumulatedProbability += change.Value;
                }
            }

            return accumulatedProbability;
        }
    }
}
