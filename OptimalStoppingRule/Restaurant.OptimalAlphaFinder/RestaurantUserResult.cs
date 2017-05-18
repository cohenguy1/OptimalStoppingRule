using GamesCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.OptimalAlphaFinder
{
    public class RestaurantUserResult : UserResult
    {
        public int?[] TurnValues = new int?[Constants.TotalRestaurantPositions];
    }
}
