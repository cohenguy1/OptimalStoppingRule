using GamesCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentsMonteCarloDecider
{
    public class Program
    {
        public static void Main(string[] args)
        {
            double[] changes = new double[Constants.TotalInvestmentsTurns];

            MonteCarlo.InitializeChangeProbabilities();

            var minProb = MonteCarlo.ChangeProbabilities.Min(kv => kv.Key);
            var maxProb = MonteCarlo.ChangeProbabilities.Max(kv => kv.Key);

            var stoppingDecision = 0;
            Random random = new Random();

            bool prevShouldAsk;
            bool shouldAsk = true;

            DateTime startTime = DateTime.Now;

            for (double i = maxProb; i >= minProb; i -= 0.01)
            {
                i = Math.Round(i, 2);
                changes[0] = i;

                DateTime now = DateTime.Now;
                prevShouldAsk = shouldAsk;
                shouldAsk = MonteCarlo.ShouldAsk(changes, stoppingDecision, random);

                var dt = DateTime.Now - now;

                if (prevShouldAsk != shouldAsk)
                {
                    Console.WriteLine(i + "," + " - ask: " + shouldAsk + " Time for calculation: " + dt.TotalSeconds);
                }

                if (!shouldAsk)
                {
                    break;
                }
            }

            var totalTime = DateTime.Now - startTime;
            Console.WriteLine("Total Time for calculation: " + totalTime.TotalSeconds);

            Console.ReadLine();
        }
    }
}
