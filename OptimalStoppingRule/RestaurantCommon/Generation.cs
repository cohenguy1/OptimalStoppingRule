using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesCommon
{
    public class Generation
    {
        public static List<int> ranks = new List<int>();

        public static List<Candidate> GenerateCandidatesForPosition()
        {
            var positionCandidates = new List<Candidate>();

            for (var candidateIndex = 0; candidateIndex < Constants.RestaurantNumOfCandidates; candidateIndex++)
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
            ranks.Clear();
            for (var index = 1; index <= Constants.RestaurantNumOfCandidates; index++)
            {
                ranks.Add(index);
            }

            var ranksRemaining = Constants.RestaurantNumOfCandidates;
            int position;

            for (var index = 0; index < Constants.RestaurantNumOfCandidates; index++)
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
