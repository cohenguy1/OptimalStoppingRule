using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteCarloDecider
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int[] accepted = new int[10];

            var stoppingDecision = 0;
            Random random = new Random();

            for (int i = 1; i <= 10; i++)
            {
                //for (int j = 1; j <= 10; j++)
                //{
                    accepted[0] = i;
                    //accepted[1] = j;

                    DateTime now = DateTime.Now;
                    bool shouldAsk = MonteCarlo.ShouldAsk(accepted, stoppingDecision, random);
                    var dt = DateTime.Now - now;

                    Console.WriteLine(i /*+ "," + j*/ + " - ask: " + shouldAsk + " Time for calculation: " + dt.TotalSeconds);

                    if (!shouldAsk)
                    {
                        break;
                    }
                //}
            }

            Console.ReadLine();
        }
    }
}
