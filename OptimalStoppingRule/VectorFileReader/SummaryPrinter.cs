﻿using RestaurantCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorFileReader
{
    public class SummaryPrinter
    {
        public static void PrintSummary(StreamWriter sw, double[] acceptedCandidatesDistribution, int numOfVectors)
        {
            sw.WriteLine();
            sw.WriteLine();

            for (int i = 0; i < acceptedCandidatesDistribution.Length; i++)
            {
                acceptedCandidatesDistribution[i] /= ((double)RestaurantConstants.TotalCandidates * numOfVectors / 100);
            }

            sw.WriteLine("Summary by Accepted:");
            sw.WriteLine();
            sw.Write("\t\t");
            for (int i = 1; i <= RestaurantConstants.TotalCandidates; i++)
            {
                sw.Write(i + "\t");
            }
            sw.WriteLine();

            sw.Write("\t\t");
            for (int i = 1; i <= RestaurantConstants.TotalCandidates; i++)
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
            for (int i = 1; i <= 10; i++)
            {
                sw.Write(i + "\t");
            }
            sw.WriteLine();

            sw.Write("\t\t");
            for (int i = 1; i <= 10; i++)
            {
                sw.Write("===\t");
            }
            sw.WriteLine();

            sw.Write("Optimal\t\t");
            for (int i = 1; i <= 10; i++)
            {
                sw.Write(optimalStopPositionAcc[i] + "\t");
            }
            sw.WriteLine();

            sw.Write("MC\t\t");
            for (int i = 1; i <= 10; i++)
            {
                sw.Write(mcStopPositionAcc[i] + "\t");
            }
            sw.WriteLine();
        }

        public static void PrintDiff(StreamWriter sw, int diff)
        {
            sw.WriteLine();
            sw.WriteLine();

            sw.WriteLine("MC and Optimal differ by " + diff + " vectors.");
        }
    }
}