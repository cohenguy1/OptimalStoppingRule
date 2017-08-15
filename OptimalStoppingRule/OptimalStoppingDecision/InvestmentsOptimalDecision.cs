using GamesCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalStoppingDecision
{
    class InvestmentsOptimalDecision
    {
        public static Dictionary<int, double> ChangeProbabilities = new Dictionary<int, double>();

        public static double[] stoppingValues = new double[Constants.TotalInvestmentsTurns];

        public const string DecisionFile = "Optimal.txt";

        public static void Main(string[] args)
        {
            if (File.Exists(DecisionFile))
            {
                File.Delete(DecisionFile);
            }

            InitializeChangeProbabilities();

            var expectation = GetExpectation(ChangeProbabilities);

            var minChange = ChangeProbabilities.Min(kv => kv.Key);
            var maxChange = ChangeProbabilities.Max(kv => kv.Key);

            // get worst probability
            stoppingValues[Constants.TotalInvestmentsTurns - 1] = minChange;

            Dictionary<int, double> expectedChangeTi = new Dictionary<int, double>();

            var Eiplus1 = GetExpectation(ChangeProbabilities);
            for (int i = Constants.TotalInvestmentsTurns - 1; i > 0; i--)
            {
                var Ti = Eiplus1;
                stoppingValues[i - 1] = Ti;

                var Ei = GetProbabilityThatChangeLess(Ti) * Eiplus1 +
                    GetExpectedChangeGreaterThanOrEquals(Ti);
                Eiplus1 = Ei;
            }

            for (int i = Constants.TotalInvestmentsTurns - 1; i >= 0; i--)
            {
                Console.WriteLine((i + 1) + " Stopping Value: " + stoppingValues[i]);
            }

            Console.WriteLine("******");

            FileStream output = new FileStream(DecisionFile, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(output);

            sw.WriteLine("Dictionary for probabilities: ");
            for (int i = 0; i < Constants.TotalInvestmentsTurns; i++)
            {
                sw.WriteLine("{" + (i + 1) + ", " + stoppingValues[i] + " }, ");
            }

            sw.Close();
            output.Close();

            Console.ReadLine();
        }

        private static void InitializeChangeProbabilities()
        {
            FileStream fs = new FileStream("NasdaqChange.txt", FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            for (int i = 0; i < Constants.InvestmentsNumOfChanges; i++)
            {
                string line = sr.ReadLine();

                var change = int.Parse(line);

                if (!ChangeProbabilities.ContainsKey(change))
                {
                    ChangeProbabilities.Add(change, 0);
                }

                ChangeProbabilities[change] += 1.0 / Constants.InvestmentsNumOfChanges;
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

        private static double GetExpectation(Dictionary<int, double> changeProbabilities)
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
