using RestaurantCommon;
using System;
using System.Collections.Generic;
using System.IO;
using MonteCarloDecider;

namespace MonteCarloReader
{
    class Program
    {
        private static int vectorNum = 0;

        public static void Main(string[] args)
        {
            FileStream fs = new FileStream("Vectors.txt", FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            Random random = new Random();
            bool terminate;

            while (vectorNum <= 50)
            {
                int[] accepted = ReadNextVector(sr, out terminate);

                if (terminate)
                {
                    break;
                }

                if (accepted[0] > 3)
                {
                    Console.Write("Vector " + vectorNum + " should stop at position ");
                    // spare the stoppingDecision = 0 since it didn't stop (accepted[0] > 3)
                    for (int stoppingDecision = 1; stoppingDecision < 10; stoppingDecision++)
                    {
                        bool shouldAsk = MonteCarlo.ShouldAsk(accepted, stoppingDecision, random);

                        if (shouldAsk)
                        {
                            Console.WriteLine(stoppingDecision);
                            break;
                        }
                    }
                }
            }

            Console.ReadLine();
        }

        public static int[] ReadNextVector(StreamReader sr, out bool terminate)
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
                    DecisionMaker.GetInstance().DetermineCandidateRank(candidatesByNow, newCandidate);

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
}
