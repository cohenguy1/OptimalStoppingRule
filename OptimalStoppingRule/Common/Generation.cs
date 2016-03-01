using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantCommon
{
    public class Generation
    {

        public static List<Candidate> GenerateCandidatesForPosition()
        {
            var positionCandidates = new List<Candidate>();

            for (var candidateIndex = 0; candidateIndex < Constants.TotalCandidates; candidateIndex++)
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
            for (var index = 1; index <= Constants.TotalCandidates; index++)
            {
                ranks.Add(index);
            }

            var ranksRemaining = Constants.TotalCandidates;
            var randomGenerator = new Random();

            for (var index = 0; index < Constants.TotalCandidates; index++)
            {
                var position = randomGenerator.Next(1, ranksRemaining + 1) - 1;

                positionCandidates[index].CandidateRank = ranks[position];

                ranks.RemoveAt(position);
                ranksRemaining--;
            }

            return positionCandidates;
        }

    }
}
