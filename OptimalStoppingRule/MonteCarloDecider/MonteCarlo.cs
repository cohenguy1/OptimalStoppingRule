using RestaurantCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteCarloDecider
{
    public class MonteCarlo
    {
        public const int NumOfVectors = 1000000;

        public const double alpha = 0.45;

        public static bool ShouldAsk(int[] accepted, int stoppingDecision, Random random)
        {
            var positionCandidates = Generation.GenerateCandidatesForPosition();
            var candidatesByNow = new List<Candidate>();

            double[] exponentialSmoothing = new double[10];
            double[] exponentialSmoothingAccumulated = new double[10];

            int[] acceptedClone = cloneArray(accepted);

            for (var positionIndex = 0; positionIndex <= stoppingDecision; positionIndex++)
            {
                if (positionIndex == 0)
                {
                    exponentialSmoothing[positionIndex] = accepted[0];
                }
                else
                {
                    exponentialSmoothing[positionIndex] = alpha * accepted[positionIndex] + (1 - alpha) * exponentialSmoothing[positionIndex - 1];
                }
            }

            for (var i = 0; i < NumOfVectors; i++)
            {
                // generate random candidates for each of the remaining positions and update the selected candidates
                for (var positionIndex = stoppingDecision + 1; positionIndex < 10; positionIndex++)
                {
                    Generation.InitCandidatesForPosition(positionCandidates, random);

                    acceptedClone[positionIndex] = SelectCandidate(positionCandidates, candidatesByNow, positionIndex, random);
                }

                // determine the exponential smoothing according to the new randomized candidates, for each position
                for (var positionIndex = stoppingDecision + 1; positionIndex < RestaurantConstants.TotalPositions; positionIndex++)
                {
                    exponentialSmoothing[positionIndex] = alpha * acceptedClone[positionIndex] + (1 - alpha) * exponentialSmoothing[positionIndex - 1];
                    exponentialSmoothingAccumulated[positionIndex] += exponentialSmoothing[positionIndex];
                }
            }

            // precalculated smooting (monte carlo doesn't affect this smoothing)
            for (var positionIndex = 0; positionIndex <= stoppingDecision; positionIndex++)
            {
                exponentialSmoothingAccumulated[positionIndex] = exponentialSmoothing[positionIndex];
            }

            for (var positionIndex = stoppingDecision + 1; positionIndex < RestaurantConstants.TotalPositions; positionIndex++)
            {
                exponentialSmoothingAccumulated[positionIndex] /= NumOfVectors;
            }

            bool foundBetter = false;
            var currentES = exponentialSmoothingAccumulated[stoppingDecision];
            for (var positionIndex = stoppingDecision + 1; positionIndex < RestaurantConstants.TotalPositions; positionIndex++)
            {
                if (exponentialSmoothingAccumulated[positionIndex] < currentES)
                {
                    foundBetter = true;
                }
            }

            return !foundBetter;
        }

        private static int SelectCandidate(List<Candidate> positionCandidates, List<Candidate> candidatesByNow, int positionIndex, Random random)
        {
            candidatesByNow.Clear();
            for (int candidateIndex = 0; candidateIndex < RestaurantConstants.TotalCandidates; candidateIndex++)
            {
                var currentCandidate = positionCandidates[candidateIndex];
                DecisionMaker.GetInstance().DetermineCandidateRank(candidatesByNow, currentCandidate, random);

                if (currentCandidate.CandidateAccepted)
                {
                    return currentCandidate.CandidateRank;
                }
            }

            return 0;
        }

        private static int[] cloneArray(int[] accepted)
        {
            int[] newArray = new int[RestaurantConstants.TotalPositions];

            for (int i = 0; i < 10; i++)
            {
                newArray[i] = accepted[i];
            }

            return newArray;
        }
    }
}
