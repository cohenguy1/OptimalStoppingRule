using GamesCommon;
using System;
using System.Collections.Generic;
using System.IO;
using VectorFileReader;
using InvestmentsMonteCarloDecider;

namespace Investments.VectorsFileReader
{
    public class InvestmentsVectorFileReader
    {
        private static int vectorNum = 0;

        public static void Main(string[] args)
        {
            FileStream fs = new FileStream("Vectors.txt", FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            Random random = new Random();

            bool terminate;

            int[] optimalStopPositionAcc = new int[Constants.TotalInvestmentsTurns + 1];
            int[] mcStopPositionAcc = new int[Constants.TotalInvestmentsTurns + 1];

            var diff = 0;

            FileStream fs2 = new FileStream("VectorsOutput.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs2);

            sw.WriteLine("Vector\tOptimal\tMC\t\tChanges");

            var startTime = DateTime.Now;
            Console.WriteLine("Started at: " + startTime);

            MonteCarlo.InitializeChangeProbabilities();

            while (vectorNum <= Constants.NumOfVectors)
            {
                Console.WriteLine("Processing Vector " + (vectorNum + 1));

                int[] changes = ReadNextVector(sr, out terminate, ref vectorNum);

                if (terminate)
                {
                    break;
                }

                if (changes[0] == 0)
                {
                    continue;
                }

                int optimalStoppingPosition = GetOptimalStopping(changes) + 1;
                optimalStopPositionAcc[optimalStoppingPosition]++;
            
                int mcStoppingPosition = GetMonteCarloStopping(changes, random) + 1;
                mcStopPositionAcc[mcStoppingPosition]++;

                if (optimalStoppingPosition != mcStoppingPosition)
                {
                    diff++;
                }

                sw.Write(vectorNum + "\t" + optimalStoppingPosition + "\t" + mcStoppingPosition + "\t\t");

                for (int index = 0; index < Constants.TotalInvestmentsTurns; index++)
                {
                    sw.Write(changes[index] + "\t");
                }

                sw.WriteLine();
            }

            SummaryPrinter.SetNumOfIterations(Constants.TotalInvestmentsTurns);

            SummaryPrinter.PrintSummary(sw, optimalStopPositionAcc, mcStopPositionAcc, "Position");

            SummaryPrinter.PrintDiff(sw, diff);

            Console.WriteLine("Finished at: " + DateTime.Now);
            Console.WriteLine("Total Time: " + (DateTime.Now - startTime).TotalMinutes + " minutes");

            sw.Close();
            sr.Close();

            fs.Close();
            fs2.Close();

            Console.ReadLine();
        }

        private static int GetOptimalStopping(int[] accepted)
        {
            int stoppingDecision = 0;

            for (; stoppingDecision < Constants.TotalInvestmentsTurns; stoppingDecision++)
            {
                bool shouldAsk = Optimal.ShouldAsk(accepted, stoppingDecision);

                if (shouldAsk)
                {
                    break;
                }
            }

            return stoppingDecision;
        }

        private static int GetMonteCarloStopping(int[] changes, Random random)
        {
            int stoppingDecision = 0;

            for (; stoppingDecision < Constants.TotalInvestmentsTurns; stoppingDecision++)
            {
                bool shouldAsk = MonteCarlo.ShouldAsk(changes, stoppingDecision, random);

                if (shouldAsk)
                {
                    break;
                }
            }

            return stoppingDecision;
        }

        public static int[] ReadNextVector(StreamReader sr, out bool terminate, ref int vectorNum)
        {
            terminate = false;

            int[] changes = new int[Constants.TotalInvestmentsTurns];
            string line = sr.ReadLine();

            if (line == null)
            {
                terminate = true;
                return changes;
            }

            while (line != null)
            {
                if (line.StartsWith("--") || line == string.Empty)
                {
                    if (line.StartsWith("--"))
                    {
                        vectorNum++;
                    }
                    else
                    {
                        break;
                    }

                    line = sr.ReadLine();
                    continue;
                }

                string[] changesStr = line.Split(' ');

                int i = 0;
                bool skipLine = false;
                foreach (string changeStr in changesStr)
                {
                    int change;
                    if (changeStr == string.Empty)
                    {
                        continue;
                    }

                    if (!int.TryParse(changeStr, out change))
                    {
                        line = sr.ReadLine();
                        skipLine = true;
                        break;
                    }

                    changes[i] = change;
                    i++;
                }

                if (skipLine)
                {
                    continue;
                }

                line = sr.ReadLine();
            }

            return changes;
        }
    }
}
