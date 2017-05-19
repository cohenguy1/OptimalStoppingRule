using GamesCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Restaurant.OptimalAlphaFinder
{
    public class AlphaFinder
    {
        public const string InputFile = "Random.txt";

        public const string OutputFile = "Alphas.txt";

        public const double delta = 0.001;

        public static void Main(string[] args)
        {
            var userResults = ResultsFileParser.Parse(InputFile);
            //userResults = userResults.Where(user => user.UserIndex % 3 == 0);

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

                if (alpha.ToString("0.00") == "0.38")
                {
                    PrintSmoothedAndRateValues(userResults);
                }
            }

            FindBestAlpha(alphaValues);

            WriteAlphasByOrder(alphaValues);

            Console.ReadLine();

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

        private static void FindBestAlpha(Dictionary<double, double> alphaValues)
        {
            var bestAlpha = 0.0;
            var bestAlphaValue = 0.0;

            foreach (var keyValue in alphaValues)
            {
                var alpha = keyValue.Key;
                var alphaValue = keyValue.Value;

                if (alphaValue < bestAlphaValue)
                {
                    bestAlpha = alpha;
                    bestAlphaValue = alphaValue;
                }
            }

            Console.WriteLine("Best Alpha: " + bestAlpha);
            Console.WriteLine("Best Pearson Correlation: " + bestAlphaValue);
        }

        public static double CaluclateExponentialSmoothedValue(int?[] turnValues, double alpha)
        {
            var exponentialSmoothedValue = (double)turnValues[0];

            for (int turnIndex = 1; turnIndex < Constants.TotalRestaurantPositions; turnIndex++)
            {
                if (turnValues[turnIndex] == null)
                {
                    if (turnIndex + 1 < Constants.TotalRestaurantPositions && turnValues[turnIndex + 1] != null)
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
