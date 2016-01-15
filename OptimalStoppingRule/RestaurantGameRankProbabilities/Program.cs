using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimalStoppingRule
{
    public class Program
    {
        public const int TotalCandidates = DecisionMaker.TotalCandidates;

        private static List<Candidate> GenerateCandidatesForPosition()
        {
            var positionCandidates = new List<Candidate>();

            for (var candidateIndex = 0; candidateIndex < TotalCandidates; candidateIndex++)
            {
                var newCandidate = new Candidate()
                {
                    CandidateState = CandidateState.New,
                    CandidateNumber = candidateIndex,
                    CandidateAccepted = false
                };

                positionCandidates.Add(newCandidate);
            }

            var ranks = new List<int>();
            for (var index = 1; index <= TotalCandidates; index++)
            {
                ranks.Add(index);
            }

            var ranksRemaining = TotalCandidates;
            var randomGenerator = new Random();

            for (var index = 0; index < TotalCandidates; index++)
            {
                var position = randomGenerator.Next(1, ranksRemaining + 1) - 1;

                positionCandidates[index].CandidateRank = ranks[position];

                ranks.RemoveAt(position);
                ranksRemaining--;
            }

            return positionCandidates;
        }

        private static void DetermineCandidateRank(List<Candidate> candidatesByNow, Candidate newCandidate)
        {
            int newCandidateIndex = 0;
            foreach (var candidate in candidatesByNow)
            {
                if (candidate.CandidateRank > newCandidate.CandidateRank)
                {
                    break;
                }

                newCandidateIndex++;
            }

            candidatesByNow.Insert(newCandidateIndex, newCandidate);

            var dm = new DecisionMaker();

            var accepted = dm.Decide(candidatesByNow, newCandidateIndex);

            newCandidate.CandidateAccepted = accepted;
        }

        public const long NumberOfTrials = 200000000;

        public static void Main(string[] args)
        {
            var acceptedCount = new long[TotalCandidates];
            var decisionMaker = new DecisionMaker();

            var candidatesByNow = new List<Candidate>();

            var startTimer = DateTime.Now;
            var timeIterationCount = 1000000;

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

                var positionCandidates = GenerateCandidatesForPosition();
                candidatesByNow.Clear();

                for (int candidateIndex = 0; candidateIndex < TotalCandidates; candidateIndex++)
                {
                    var currentCandidate = positionCandidates[candidateIndex];
                    DetermineCandidateRank(candidatesByNow, currentCandidate);

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
