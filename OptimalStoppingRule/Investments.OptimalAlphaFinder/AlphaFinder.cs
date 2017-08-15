using GamesCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investments.OptimalAlphaFinder
{
    public class AlphaFinder
    {
        public const string InputFile = "Random.txt";

        public const string OutputFile = "Alphas.txt";

        public const string AlphasByObservationsFile = "AlphasByObservations.txt";

        public const double delta = 0.001;

        public static void Main(string[] args)
        {
            var userResultsSize = ResultsFileParser.Parse(InputFile).Count();
            Dictionary<int, double> alphaByNumOfObservations = new Dictionary<int, double>();
            var random = new Random();
            for (int randomSize = 2; randomSize <= userResultsSize; randomSize += 2)
            {
                var userResults = ResultsFileParser.Parse(InputFile);
                double bestAlphaAverage = 0.0;
                for (int j = 0; j < Constants.NumOfPermutations; j++)
                {
                    var randomUserResults = chooseRandomUserResults(userResults, randomSize, random);
                    var bestAlpha = CalculateCorrelationsAndFindBestAlpha(randomUserResults);
                    bestAlphaAverage += bestAlpha;
                }
                bestAlphaAverage /= Constants.NumOfPermutations;

                Console.WriteLine("NumOfObservations: " + randomSize + "  Best Alpha " + bestAlphaAverage);

                alphaByNumOfObservations.Add(randomSize, bestAlphaAverage);
            }

            WriteAlphasToFile(alphaByNumOfObservations);
            //WriteAlphasByOrder(alphaValues);

            Console.ReadLine();

        }

        private static List<InvestmentUserResult> chooseRandomUserResults(IEnumerable<InvestmentUserResult> userResults, int randomSize, Random random)
        {
            var randomUserResultsRemaining = new List<InvestmentUserResult>(userResults);
            var randomUserResults = new List<InvestmentUserResult>();

            for (int i = 0; i < randomSize; i++)
            {
                var resultPosition = random.Next(randomUserResultsRemaining.Count);
                var userResult = randomUserResultsRemaining[resultPosition];

                randomUserResultsRemaining.Remove(userResult);
                randomUserResults.Add(userResult);
            }

            return randomUserResults;
        }

        private static double CalculateCorrelationsAndFindBestAlpha(IEnumerable<InvestmentUserResult> userResults)
        {
            var alphaValues = new Dictionary<double, double>();

            for (var alpha = 0.0; alpha <= 1.0; alpha += delta)
            {
                int count = 0;

                foreach (var user in userResults)
                {
                    count++;
                    user.ExponentialSmoothedValue = CaluclateExponentialSmoothedValue(user.TurnValues, alpha);
                }

                var correlation = CorrelationFinder.GetCorrelation(userResults);

                alphaValues.Add(alpha, correlation);
            }

            return FindBestAlpha(alphaValues);
        }

        private static void PrintSmoothedAndRateValues(IEnumerable<UserResult> userResults)
        {
            FileStream fs = new FileStream("CorrelationTest.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            foreach (var user in userResults)
            {
                sw.WriteLine(user.ExponentialSmoothedValue.ToString("0.000"));
            }

            sw.WriteLine("");
            sw.WriteLine("###########################3");
            sw.WriteLine("");

            foreach (var user in userResults)
            {
                sw.WriteLine(user.AdviserRating);
            }

            sw.Close();
            fs.Close();
        }

        public static void WriteAlphasToFile(Dictionary<int, double> alphasByObservations)
        {
            var sorted = alphasByObservations.OrderBy(pair => pair.Key);
            FileStream fs = new FileStream(AlphasByObservationsFile, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            foreach (var keyValue in sorted)
            {
                sw.WriteLine(keyValue.Key + " " + keyValue.Value.ToString("0.000"));
            }

            sw.Close();
            fs.Close();
        }

        private static void WriteAlphasByOrder(Dictionary<double, double> alphaValues)
        {
            var sorted = alphaValues.OrderByDescending(pair => pair.Value);
            FileStream fs = new FileStream(OutputFile, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            foreach (var keyValue in sorted)
            {
                sw.WriteLine("Alpha: " + keyValue.Key.ToString("0.000") + "  /  " + keyValue.Value);
            }

            sw.Close();
            fs.Close();
        }

        private static double FindBestAlpha(Dictionary<double, double> alphaValues)
        {
            var bestAlpha = 0.0;
            var bestAlphaValue = 0.0;

            FileStream alphaVsPearsonFile = new FileStream("AlphaVsPearson.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(alphaVsPearsonFile);

            foreach (var keyValue in alphaValues)
            {
                var alpha = keyValue.Key;
                var alphaValue = keyValue.Value;

                if (alphaValue > bestAlphaValue)
                {
                    bestAlpha = alpha;
                    bestAlphaValue = alphaValue;
                }

                double temp = Math.Round(alpha * 1000);
                if ((int)temp % 10 == 0)
                {
                    sw.WriteLine(alpha.ToString("0.00") + " , " + alphaValue.ToString());
                }
            }

            sw.Close();
            alphaVsPearsonFile.Close();

            //Console.WriteLine("Best Alpha: " + bestAlpha);
            //Console.WriteLine("Best Pearson Correlation: " + bestAlphaValue);

            return bestAlpha;
        }

        public static double CaluclateExponentialSmoothedValue(int?[] turnValues, double alpha)
        {
            var exponentialSmoothedValue = (double)turnValues[0];

            for (int turnIndex = 1; turnIndex < Constants.TotalInvestmentsTurns; turnIndex++)
            {
                if (turnValues[turnIndex] == null)
                {
                    if (turnIndex + 1 < Constants.TotalInvestmentsTurns && turnValues[turnIndex + 1] != null)
                    {
                        throw new NullReferenceException();
                    }

                    break;
                }

                var turnValue = (double)turnValues[turnIndex];

                exponentialSmoothedValue = alpha * turnValue + (1 - alpha) * exponentialSmoothedValue;
            }

            return exponentialSmoothedValue;
        }
    }
}
