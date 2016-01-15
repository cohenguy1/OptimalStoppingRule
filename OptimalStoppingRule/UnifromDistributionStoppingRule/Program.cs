using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnifromDistributionStoppingRule
{
    class Program
    {
        public const int Samples = 20;

        public static double[] stoppingValues = new double[Samples];

        public static double[] values = new double[Samples];

        public static void Main(string[] args)
        {
            stoppingValues[Samples - 1] = 0;

            for (int i = Samples - 2; i >= 0; i--)
            {
                stoppingValues[i] = 50 + 1 / (double)200 * (double)Math.Pow(stoppingValues[i + 1], 2);
            }

            for (int i = Samples - 1; i >= 0; i--)
            {
                Console.WriteLine((i + 1) + " Stopping Value: " + stoppingValues[i]);
            }

            Console.WriteLine("******");

            Random r = new Random();

            for (int i = 0; i < Samples; i++)
            {
                values[i] = r.NextDouble() * (double)100;

                if (values[i] > stoppingValues[i])
                {
                    Console.WriteLine("Stopped at: " + (i + 1) + " Value: " + values[i]);
                    break;
                }
            }

            Console.ReadLine();
        }
    }
}
