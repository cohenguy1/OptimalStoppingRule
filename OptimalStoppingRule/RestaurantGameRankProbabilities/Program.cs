using RestaurantCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OptimalStoppingRule
{
    public class Program
    {
        public const int TotalCandidates = Constants.TotalCandidates;

        public static DecisionMaker dm = new DecisionMaker();

        
        public const long NumberOfTrials = 36000;

        public static void Main(string[] args)
        {
            var acceptedCount = new long[TotalCandidates];
            var decisionMaker = new DecisionMaker();

            var candidatesByNow = new List<Candidate>();

            var startTimer = DateTime.Now;
            var timeIterationCount = 1500;

            var positionCandidates = Generation.GenerateCandidatesForPosition();

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

                }

                Thread.Sleep(25);
                Generation.InitCandidatesForPosition(positionCandidates);
                candidatesByNow.Clear();

                for (int candidateIndex = 0; candidateIndex < TotalCandidates; candidateIndex++)
                {
                    var currentCandidate = positionCandidates[candidateIndex];
                    DecisionMaker.DetermineCandidateRank(candidatesByNow, currentCandidate);

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

                Console.WriteLine("Accepted Rank " + (i + 1) + ": " + acceptedRankProbability[i]);
            }

            Console.ReadLine();
        }
    }
}
