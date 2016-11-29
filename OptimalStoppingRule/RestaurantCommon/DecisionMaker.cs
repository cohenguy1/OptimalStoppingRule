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

        private static DecisionMaker _instance;

        public static DecisionMaker GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DecisionMaker();
            }

            return _instance;
        }

        private DecisionMaker()
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

           
            for (var i = 2; i <= TotalCandidates; i++)
            {
                StoppingRule[i] = Math.Min(Constants.TotalCandidates, StoppingRule[i] + 1);
                
                if (i >= 4)
                {
                    StoppingRule[i] = Math.Min(Constants.TotalCandidates, StoppingRule[i] + 1);
                }
            }

            var x = 3;
        }

        public void DetermineCandidateRank(List<Candidate> candidatesByNow, Candidate newCandidate, Random rand)
        {
            int newCandidateIndex = InsertNewCandidate(candidatesByNow, newCandidate);

            var accepted = Decide(candidatesByNow, newCandidateIndex, rand);

            newCandidate.CandidateAccepted = accepted;
        }

        private int InsertNewCandidate(List<Candidate> candidatesByNow, Candidate newCandidate)
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


        public bool Decide(List<Candidate> candidatesByNow, int newCandidateIndex, Random rand)
        {
            if (candidatesByNow.Count == Constants.TotalCandidates)
            {
                return true;
            }

            return (newCandidateIndex + 1 <= StoppingRule[candidatesByNow.Count]);
        }
    }
}