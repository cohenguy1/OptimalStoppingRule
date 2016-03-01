using RestaurantCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantCommon
{
    public class DecisionMaker
    {
        public const int TotalCandidates = Constants.TotalCandidates;

        private static double[] c = new double[TotalCandidates + 1];

        public static int[] StoppingRule = new int[TotalCandidates + 1];

        public DecisionMaker()
        {
            int n = TotalCandidates;

            c[n - 1] = (n + 1) / 2.0;

            StoppingRule[n] = n;
            StoppingRule[n - 1] = (int)Math.Floor((n - 1 + 1) / ((double)n + 1) * c[n - 1]);

            for (var i = n - 1; i >= 1; i--)
            {
                c[i - 1] = 1 / (double)(i) * (((n + 1) / (double)(i + 1)) * (StoppingRule[i] * (StoppingRule[i] + 1)) / 2.0 + (i - StoppingRule[i]) * c[i]);
                StoppingRule[i - 1] = (int)Math.Floor((i) / ((double)n + 1) * c[i - 1]);
            }
        }

        public static void DetermineCandidateRank(List<Candidate> candidatesByNow, Candidate newCandidate)
        {
            int newCandidateIndex = InsertNewCandidate(candidatesByNow, newCandidate);

            var accepted = Decide(candidatesByNow, newCandidateIndex);

            newCandidate.CandidateAccepted = accepted;
        }

        private static int InsertNewCandidate(List<Candidate> candidatesByNow, Candidate newCandidate)
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
            return newCandidateIndex;
        }


        public static bool Decide(List<Candidate> candidatesByNow, int newCandidateIndex)
        {
            return (newCandidateIndex + 1 <= StoppingRule[candidatesByNow.Count]);
        }

        private bool Decide2(List<Candidate> candidatesByNow, Candidate newCandidate)
        {
            if (candidatesByNow.Count <= (int)Math.Sqrt(TotalCandidates))
            {
                return false;
            }

            if (candidatesByNow.Count == TotalCandidates - 1)
            {
                return true;
            }

            var firstSqrtCandidates = candidatesByNow.Where(candidate => candidate.CandidateNumber < Math.Sqrt(TotalCandidates));

            var minRank = firstSqrtCandidates.Min(candidate => candidate.CandidateRank);

            return (newCandidate.CandidateRank < minRank);
        }
    }
}