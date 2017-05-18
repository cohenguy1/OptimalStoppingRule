using GamesCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesCommon
{
    public class SummaryPrinter
    {
        public static int NumOfIterations;

        public static void PrintSummary(StreamWriter sw, double[] acceptedCandidatesDistribution, int numOfVectors)
        {
            sw.WriteLine();
            sw.WriteLine();

            for (int i = 0; i < acceptedCandidatesDistribution.Length; i++)
            {
                acceptedCandidatesDistribution[i] /= ((double)Constants.RestaurantNumOfCandidates * numOfVectors / 100);
            }

            sw.WriteLine("Summary by Accepted:");
            sw.WriteLine();
            sw.Write("\t\t");
            for (int i = 1; i <= Constants.RestaurantNumOfCandidates; i++)
            {
                sw.Write(i + "\t");
            }
            sw.WriteLine();

            sw.Write("\t\t");
            for (int i = 1; i <= Constants.RestaurantNumOfCandidates; i++)
            {
                sw.Write("===\t");
            }
            sw.WriteLine();

            sw.Write("\t\t");
            for (int i = 1; i <= 10; i++)
            {
                sw.Write(acceptedCandidatesDistribution[i].ToString("0.00") + "\t");
            }
            sw.WriteLine();
        }

        public static void PrintSummary(StreamWriter sw, int[] optimalStopPositionAcc, int[] mcStopPositionAcc, string aspect)
        {
            sw.WriteLine();
            sw.WriteLine();

            sw.WriteLine("Summary by " + aspect);
            sw.WriteLine();
            sw.Write("\t\t");
            for (int i = 1; i <= NumOfIterations; i++)
            {
                sw.Write(i + "\t");
            }
            sw.WriteLine();

            sw.Write("\t\t");
            for (int i = 1; i <= NumOfIterations; i++)
            {
                sw.Write("===\t");
            }
            sw.WriteLine();

            sw.Write("Optimal\t\t");
            for (int i = 1; i <= NumOfIterations; i++)
            {
                sw.Write(optimalStopPositionAcc[i] + "\t");
            }
            sw.WriteLine();

            sw.Write("MC\t\t");
            for (int i = 1; i <= NumOfIterations; i++)
            {
                sw.Write(mcStopPositionAcc[i] + "\t");
            }
            sw.WriteLine();
        }

        public static void SetNumOfIterations(int totalInvestmentsTurns)
        {
            NumOfIterations = totalInvestmentsTurns;
        }

        public static void PrintDiff(StreamWriter sw, IEnumerable<int> similarVectors)
        {
            sw.WriteLine();
            sw.WriteLine();

            sw.WriteLine("MC and Optimal differ by " + (Constants.NumOfVectors - similarVectors.Count()) + " vectors.");
            sw.WriteLine("Similar Vectors:");
            foreach (var vector in similarVectors)
            {
                sw.Write(vector + " ");
            }

        }
    }
}
