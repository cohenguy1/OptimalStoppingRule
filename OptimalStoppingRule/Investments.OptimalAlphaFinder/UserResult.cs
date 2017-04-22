using GamesCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investments.OptimalAlphaFinder
{
    public class UserResult
    {
        public string UserId;

        public string AskHeuristics;

        public int AdviserRating;

        public int?[] TurnValues = new int?[Constants.TotalInvestmentsTurns];

        public double ExponentialSmoothedValue;

        public int UserIndex;
    }
}
