using GamesCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investments.OptimalAlphaFinder
{
    public class InvestmentUserResult : UserResult
    {
        public int?[] TurnValues = new int?[Constants.TotalInvestmentsTurns];
    }
}
