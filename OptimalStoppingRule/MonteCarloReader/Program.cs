using RestaurantCommon;
using System;
using System.Collections.Generic;
using System.IO;
using MonteCarloDecider;
using VectorFileReader;

namespace MonteCarloReader
{
    class Program
    {
        private static int vectorNum = 0;

        public static void Main(string[] args)
        {
            FileStream fs = new FileStream("Vectors.txt", FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            Random random = new Random();
            bool terminate;

            while (vectorNum <= 50)
            {
                int[] accepted = RestaurantVectorFileReader.ReadNextVector(sr, out terminate, ref vectorNum);

                if (terminate)
                {
                    break;
                }

                if (accepted[0] > 3)
                {
                    Console.Write("Vector " + vectorNum + " should stop at position ");
                    // spare the stoppingDecision = 0 since it didn't stop (accepted[0] > 3)
                    for (int stoppingDecision = 1; stoppingDecision < 10; stoppingDecision++)
                    {
                        bool shouldAsk = MonteCarlo.ShouldAsk(accepted, stoppingDecision, random);

                        if (shouldAsk)
                        {
                            Console.WriteLine(stoppingDecision);
                            break;
                        }
                    }
                }
            }

            Console.ReadLine();
        }

        
    }
}
