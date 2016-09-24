using RestaurantCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OptimalStoppingRule
{
    public class Program
    {
        public const int TotalCandidates = Constants.TotalCandidates;

        public const string ProbabilitiesFile = "Probabilities.txt";

        public const long NumberOfTrials = 10000000;

        public static void Main(string[] args)
        {
            if (File.Exists(ProbabilitiesFile))
            {
                File.Delete(ProbabilitiesFile);
            }

            FileStream output = new FileStream(ProbabilitiesFile, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(output);

            var acceptedCount = new long[TotalCandidates];

            var candidatesByNow = new List<Candidate>();

            var startTimer = DateTime.Now;
            var timeIterationCount = NumberOfTrials / 1000;

            var positionCandidates = Generation.GenerateCandidatesForPosition();

            double[] validation = new double[10];

            Random random = new Random();

            for (long index = 0; index < NumberOfTrials; index++)
            {
                if (index == timeIterationCount)
                {
                    Console.WriteLine("Time: " + DateTime.Now + " precent: " + index / (double)NumberOfTrials);

                    var loopsCount = NumberOfTrials / (double)timeIterationCount;
                    var timeForLoop = DateTime.Now - startTimer;

                    TimeSpan timeRemiaiming = new TimeSpan();

                    for (int i = 0; i < loopsCount; i++)
                    {
                        timeRemiaiming += timeForLoop;

                    }

                    DateTime eta = startTimer + timeRemiaiming;

                    Console.WriteLine("ETA for completion: " + eta);

                    Thread.Sleep(25);
                }
                
                Generation.InitCandidatesForPosition(positionCandidates, random);
                candidatesByNow.Clear();

                for (int candidateIndex = 0; candidateIndex < TotalCandidates; candidateIndex++)
                {
                    validation[positionCandidates[candidateIndex].CandidateRank - 1] += candidateIndex + 1;
                }

                for (int candidateIndex = 0; candidateIndex < TotalCandidates; candidateIndex++)
                {
                    var currentCandidate = positionCandidates[candidateIndex];
                    DecisionMaker.GetInstance().DetermineCandidateRank(candidatesByNow, currentCandidate);

                    if (currentCandidate.CandidateAccepted)
                    {
                        acceptedCount[currentCandidate.CandidateRank - 1]++;
                        break;
                    }
                }
            }

            var acceptedRankProbability = new double[TotalCandidates];

            for (int i = 0; i < TotalCandidates; i++)
            {
                acceptedRankProbability[i] = acceptedCount[i] / (double)NumberOfTrials;

                sw.WriteLine("Accepted Rank " + (i + 1) + ": " + acceptedRankProbability[i]);
            }

            for (int i = 0; i < TotalCandidates; i++)
            {
                validation[i] /= NumberOfTrials;

                sw.WriteLine("Validation " + (i + 1) + ": " + validation[i]);
            }

            sw.Close();
            output.Close();

            Console.WriteLine("Completed! Press any key to exit...");
            Console.ReadLine();
        }
    }
}
