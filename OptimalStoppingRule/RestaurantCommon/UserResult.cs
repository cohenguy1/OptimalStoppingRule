using GamesCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesCommon
{
    public abstract class UserResult
    {
        public string UserId;

        public string AskHeuristics;

        public int AdviserRating;

        public double ExponentialSmoothedValue;

        public int UserIndex;
    }
}
