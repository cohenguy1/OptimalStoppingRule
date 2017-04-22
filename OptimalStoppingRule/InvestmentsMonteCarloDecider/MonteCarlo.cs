using GamesCommon;
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
        public static Dictionary<int, double> ChangeProbabilities = new Dictionary<int, double>();

        public static int[] ChangeProbabilitiesArray = new int[Constants.InvestmentsNumOfChanges];

        public const int NumOfVectors = 1 * 1000 * 1000;

        public const double alpha = 0.66;

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


        public static bool ShouldAsk(int[] changes, int stoppingDecision, Random random)
        {
            double[] exponentialSmoothing = new double[Constants.TotalInvestmentsTurns];
            double[] exponentialSmoothingAccumulated = new double[Constants.TotalInvestmentsTurns];

            int[] stoppingCount = new int[Constants.TotalInvestmentsTurns];

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

            var count = 0;
            double avg = 0;
            for (var i = 0; i < NumOfVectors; i++)
            {
                for (var turnIndex = stoppingDecision + 1; turnIndex < Constants.TotalInvestmentsTurns; turnIndex++)
                {
                    var randomChange = GetRandomChange(random);
                    avg += randomChange;
                    count++;

                    // determine the exponential smoothing according to the new randomized turns
                    exponentialSmoothing[turnIndex] = alpha * randomChange + (1 - alpha) * exponentialSmoothing[turnIndex - 1];
                    //exponentialSmoothingAccumulated[turnIndex] += exponentialSmoothing[turnIndex];
                }

                double curExp = exponentialSmoothing[stoppingDecision];

                double maxExp = exponentialSmoothing[stoppingDecision + 1];
                int maxIndex = stoppingDecision + 1;
                for (var positionIndex = stoppingDecision + 1; positionIndex < 10; positionIndex++)
                {
                    if (exponentialSmoothing[positionIndex] > maxExp)
                    {
                        maxExp = exponentialSmoothing[positionIndex];
                        maxIndex = positionIndex;
                    }
                }

                if (maxExp > curExp)
                {
                    stoppingCount[maxIndex]++;
                }
                else
                {
                    stoppingCount[stoppingDecision]++;
                }
            }

            if (stoppingCount.Max() == stoppingCount[stoppingDecision])
            {
                return true;
            }

            return false;
        

            avg = avg / count;
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
                randomChanges[turnIndex] = ChangeProbabilitiesArray[random.Next(Constants.InvestmentsNumOfChanges)];
            }
        }

        private static int GetRandomChange(Random random)
        {
            var changeIndex = random.Next(Constants.InvestmentsNumOfChanges);

            return ChangeProbabilitiesArray[changeIndex];
        }
    }
}
