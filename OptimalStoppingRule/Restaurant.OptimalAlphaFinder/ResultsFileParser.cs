using GamesCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.OptimalAlphaFinder
{
    public class ResultsFileParser
    {
        public static IEnumerable<RestaurantUserResult> Parse(string filePath)
        {
            var userResults = new List<RestaurantUserResult>();
            int userIndex = 0;
            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                string[] columns = line.Split('\t');

                if (columns[0] == "UserId" || string.IsNullOrEmpty(columns[0]))
                {
                    continue;
                }

                RestaurantUserResult userResult = new RestaurantUserResult();

                userResult.UserIndex = userIndex;
                userResult.UserId = columns[0];
                userResult.AskHeuristics = columns[1];
                userResult.AdviserRating = int.Parse(columns[2]);

                for (int turnIndex = 0; turnIndex < Constants.TotalRestaurantPositions; turnIndex++)
                {
                    int? turnValue = null;
                    string turnString = columns[turnIndex + 3];
                    if (!string.IsNullOrEmpty(turnString))
                    {
                        turnValue = int.Parse(turnString);
                    }

                    userResult.TurnValues[turnIndex] = turnValue;
                }

                userResults.Add(userResult);

                userIndex++;
            }

            return userResults;
        }

    }
}
