using RestaurantCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentsMonteCarloDecider
{
    public class MonteCarlo
    {
        public static Dictionary<double, double> ChangeProbabilities = new Dictionary<double, double>();

        public static double[] ChangeProbabilitiesArray = new double[Constants.NumOfChanges];

        public const int NumOfVectors = 1000000;

        public const double alpha = 0.45;

        public static void InitializeChangeProbabilities()
        {
            FileStream fs = new FileStream("NasdaqChange.txt", FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            for (int i = 0; i < Constants.NumOfChanges; i++)
            {
                string line = sr.ReadLine();

                var change = double.Parse(line);

                change = Math.Round(change, 2);

                ChangeProbabilitiesArray[i] = change;

                if (!ChangeProbabilities.ContainsKey(change))
                {
                    ChangeProbabilities.Add(change, 0);
                }

                ChangeProbabilities[change] += 1.0 / Constants.NumOfChanges;
            }
        }


        public static bool ShouldAsk(double[] changes, int stoppingDecision, Random random)
        {
            double[] exponentialSmoothing = new double[Constants.TotalInvestmentsTurns];
            double[] exponentialSmoothingAccumulated = new double[Constants.TotalInvestmentsTurns];

            for (var turnsIndex = 0; turnsIndex <= stoppingDecision; turnsIndex++)
            {
                if (turnsIndex == 0)
                {
                    exponentialSmoothing[turnsIndex] = changes[0];
                }
                else
                {
                    exponentialSmoothing[turnsIndex] = alpha * changes[turnsIndex] + (1 - alpha) * exponentialSmoothing[turnsIndex - 1];
                }
            }

            for (var i = 0; i < NumOfVectors; i++)
            {
                for (var turnIndex = stoppingDecision + 1; turnIndex < Constants.TotalInvestmentsTurns; turnIndex++)
                {
                    var randomChange = GetRandomChange(random);

                    // determine the exponential smoothing according to the new randomized turns
                    exponentialSmoothing[turnIndex] = alpha * randomChange + (1 - alpha) * exponentialSmoothing[turnIndex - 1];
                    exponentialSmoothingAccumulated[turnIndex] += exponentialSmoothing[turnIndex];
                }
            }

            // precalculated smooting (monte carlo doesn't affect this smoothing)
            for (var turnIndex = 0; turnIndex <= stoppingDecision; turnIndex++)
            {
                exponentialSmoothingAccumulated[turnIndex] = exponentialSmoothing[turnIndex];
            }

            for (var turnIndex = stoppingDecision + 1; turnIndex < Constants.TotalInvestmentsTurns; turnIndex++)
            {
                exponentialSmoothingAccumulated[turnIndex] /= NumOfVectors;
            }

            bool foundBetter = false;
            var currentES = exponentialSmoothingAccumulated[stoppingDecision];
            for (var turnIndex = stoppingDecision + 1; turnIndex < Constants.TotalInvestmentsTurns; turnIndex++)
            {
                if (exponentialSmoothingAccumulated[turnIndex] > currentES)
                {
                    foundBetter = true;
                }
            }

            return !foundBetter;
        }

        private static void DetermineRandomChanges(double[] randomChanges, int stoppingDecision, Random random)
        {
            for (var turnIndex = stoppingDecision + 1; turnIndex < Constants.TotalInvestmentsTurns; turnIndex++)
            {
                randomChanges[turnIndex] = ChangeProbabilitiesArray[random.Next(Constants.NumOfChanges)];
            }
        }

        private static double GetRandomChange(Random random)
        {
            var changeIndex = random.Next(Constants.NumOfChanges);

            return ChangeProbabilitiesArray[changeIndex];
        }
    }
}
