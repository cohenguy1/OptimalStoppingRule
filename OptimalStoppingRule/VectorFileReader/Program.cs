using MonteCarloDecider;
using OptimalStoppingRule;
using RestaurantCommon;
using System;
using System.Collections.Generic;
using System.IO;

namespace VectorFileReader
{
    public class RestaurantVectorFileReader
    {
        private static int vectorNum = 0;

        public static void Main(string[] args)
        {
            FileStream fs = new FileStream("Vectors.txt", FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            Random random = new Random();
            Random rand2 = new Random();

            bool terminate;

            double[] acceptedCandidatesDistribution = new double[11];

            int[] optimalStopRankingAcc = new int[11];
            int[] mcStopRankingAcc = new int[11];

            int[] optimalStopPositionAcc = new int[11];
            int[] mcStopPositionAcc = new int[11];

            var diff = 0;

            FileStream fs2 = new FileStream("VectorsOutput.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs2);

            sw.WriteLine("Vector\tOptimal\tMC\t\tAccepted");

            var startTime = DateTime.Now;
            Console.WriteLine("Started at: " + startTime);

            while (vectorNum <= 50)
            {
                int[] accepted = ReadNextVector(sr, out terminate, ref vectorNum, rand2);

                if (terminate)
                {
                    break;
                }

                if (accepted[0] == 0)
                {
                    continue;
                }

                int optimalStoppingPosition = GetOptimalStopping(accepted) + 1;
                optimalStopPositionAcc[optimalStoppingPosition]++;
                optimalStopRankingAcc[accepted[optimalStoppingPosition - 1]]++;

                int mcStoppingPosition = GetMonteCarloStopping(accepted, random) + 1;
                mcStopPositionAcc[mcStoppingPosition]++;
                mcStopRankingAcc[accepted[mcStoppingPosition - 1]]++;

                if (optimalStoppingPosition != mcStoppingPosition)
                {
                    diff++;
                }

                sw.Write(vectorNum + "\t" + optimalStoppingPosition + "\t" + mcStoppingPosition + "\t\t");

                for (int index = 0; index < Constants.TotalCandidates; index++)
                {
                    acceptedCandidatesDistribution[accepted[index]]++;
                    sw.Write(accepted[index] + "\t");
                }

                sw.WriteLine();
            }

            sw.WriteLine();
            sw.WriteLine();

            PrintSummary(sw, acceptedCandidatesDistribution, vectorNum);

            sw.WriteLine();
            sw.WriteLine();

            PrintSummary(sw, optimalStopPositionAcc, mcStopPositionAcc, "Position");

            sw.WriteLine();
            sw.WriteLine();
            PrintSummary(sw, optimalStopRankingAcc, mcStopRankingAcc, "Ranking");

            sw.WriteLine();
            sw.WriteLine();

            sw.WriteLine("MC and Optimal differ by " + diff + " vectors.");

            Console.WriteLine("Finished at: " + DateTime.Now);
            Console.WriteLine("Total Time: " + (DateTime.Now - startTime).TotalMinutes + " minutes");

            sw.Close();
            sr.Close();

            fs.Close();
            fs2.Close();

            Console.ReadLine();
        }

        private static void PrintSummary(StreamWriter sw, double[] acceptedCandidatesDistribution, int numOfVectors)
        {
            for (int i = 0; i < acceptedCandidatesDistribution.Length; i++)
            {
                acceptedCandidatesDistribution[i] /= ((double)Constants.TotalCandidates * numOfVectors / 100);
            }

            sw.WriteLine("Summary by Accepted:");
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

            sw.Write("\t\t");
            for (int i = 1; i <= 10; i++)
            {
                sw.Write(acceptedCandidatesDistribution[i].ToString("0.00") + "\t");
            }
            sw.WriteLine();
        }

        private static void PrintSummary(StreamWriter sw, int[] optimalStopPositionAcc, int[] mcStopPositionAcc, string aspect)
        {
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

        private static int GetOptimalStopping(int[] accepted)
        {
            int stoppingDecision = 0;

            for (; stoppingDecision < 10; stoppingDecision++)
            {
                bool shouldAsk = Optimal.ShouldAsk(accepted, stoppingDecision);

                if (shouldAsk)
                {
                    break;
                }
            }

            return stoppingDecision;
        }

        private static int GetMonteCarloStopping(int[] accepted, Random random)
        {
            if (accepted[0] <= 3)
            {
                return 0;
            }

            int stoppingDecision = 1;
            // spare the stoppingDecision = 0 since it didn't stop (accepted[0] > 3)
            for (; stoppingDecision < 10; stoppingDecision++)
            {
                bool shouldAsk = MonteCarlo.ShouldAsk(accepted, stoppingDecision, random);

                if (shouldAsk)
                {
                    break;
                }
            }

            return stoppingDecision;
        }

        public static int[] ReadNextVector(StreamReader sr, out bool terminate, ref int vectorNum, Random rand)
        {
            terminate = false;

            var positionNumber = 0;

            int[] accepted = new int[10];
            int[] intRanks = new int[Constants.TotalCandidates];
            string line = sr.ReadLine();

            if (line == null)
            {
                terminate = true;
                return accepted;
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
                    positionNumber = 0;
                    continue;
                }

                string[] ranks = line.Split(' ');

                int i = 0;
                bool skipLine = false;
                foreach (string rank in ranks)
                {
                    int intRank;
                    if (rank == string.Empty)
                    {
                        continue;
                    }

                    if (!int.TryParse(rank, out intRank))
                    {
                        line = sr.ReadLine();
                        skipLine = true;
                        break;
                    }

                    intRanks[i] = intRank;
                    i++;
                }

                if (skipLine)
                {
                    continue;
                }

                var candidatesByNow = new List<Candidate>();
                int chosenRank = 0;
                for (i = 0; i < intRanks.Length; i++)
                {
                    var newCandidate = new Candidate() { CandidateRank = intRanks[i] };
                    DecisionMaker.GetInstance().DetermineCandidateRank(candidatesByNow, newCandidate, rand);

                    if (newCandidate.CandidateAccepted)
                    {
                        chosenRank = intRanks[i];
                        accepted[positionNumber] = chosenRank;
                        break;
                    }
                }

                positionNumber++;

                line = sr.ReadLine();
            }

            return accepted;
        }

    }

    public class Optimal
    {
        private static Dictionary<int, int> minimalRankForAsk = new Dictionary<int, int>()
        {
            {10, 10 },
            {9, 4 },
            {8, 2 },
            {7, 1 },
            {6, 1 },
            {5, 1 },
            {4, 1 },
            {3, 1 },
            {2, 1 },
            {1, 1 }
        };

        public static bool ShouldAsk(int[] accepted, int stoppingDecision)
        {
            var ask = accepted[stoppingDecision] <= minimalRankForAsk[stoppingDecision + 1];
            return ask;
        }
    }
}
