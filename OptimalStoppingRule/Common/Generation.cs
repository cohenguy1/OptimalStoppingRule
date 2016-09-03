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
                    CandidateAccepted = false
                };

                positionCandidates.Add(newCandidate);
            }

            return positionCandidates;
        }

        public static void InitCandidatesForPosition(List<Candidate> positionCandidates, Random randomGenerator)
        {
            var ranks = new List<int>();
            for (var index = 1; index <= Constants.TotalCandidates; index++)
            {
                ranks.Add(index);
            }

            var ranksRemaining = Constants.TotalCandidates;
            int position;

            for (var index = 0; index < Constants.TotalCandidates; index++)
            {
                if (ranksRemaining > 1)
                {
                    position = randomGenerator.Next(1, ranksRemaining + 1) - 1;
                    //Console.Write(position + " ");
                }
                else
                {
                    position = 0;
                }
                positionCandidates[index].CandidateRank = ranks[position];
                positionCandidates[index].CandidateAccepted = false;

                ranks.RemoveAt(position);
                ranksRemaining--;
            }
            //Console.WriteLine();
        }

    }
}
