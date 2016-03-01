using OptimalStoppingRule;
using RestaurantCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantSingleRun
{
    class Program
    {
        public static void Main(string[] args)
        {
            FileStream fs = new FileStream("Vectors.txt", FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            FileStream fs2 = new FileStream("VectorsDecisions.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs2);

            DecisionMaker dm = new DecisionMaker();
            int[] intRanks = new int[20];

            double[] probs = new double[21];

            double[] freqsFirst = new double[21];
            double[] freqsLast = new double[21];

            int positionNumber = 0;

            string line = sr.ReadLine() ;
            while (line != null)
            {
                if (line.StartsWith("--") || line == string.Empty)
                {
                    sw.WriteLine(line);
                    line = sr.ReadLine();
                    positionNumber = 0;
                    continue;
                }

                positionNumber++;

                string[] ranks = line.Split(' ');

                int i = 0;
                bool skipLine = false;
                foreach (string rank in ranks)
                {
                    int intRank;
                    if (rank == string.Empty)
                    {
                        continue;
                    }

                    if (!int.TryParse(rank, out intRank))
                    {
                        sw.WriteLine(line);
                        line = sr.ReadLine();
                        skipLine = true;
                        break;
                    }

                    intRanks[i] = intRank;
                    i++;
                }

                if (skipLine)
                {
                    continue;
                }

                var candidatesByNow = new List<Candidate>();
                int chosenRank = 0;
                for (i = 0; i < intRanks.Length; i++)
                {
                    var newCandidate = new Candidate() { CandidateRank = intRanks[i] };
                    DecisionMaker.DetermineCandidateRank(candidatesByNow, newCandidate);

                    if (newCandidate.CandidateAccepted)
                    {
                        chosenRank = intRanks[i];
                        probs[chosenRank]++;

                        if (positionNumber == 1)
                        {
                            freqsFirst[chosenRank]++;
                        }
                        else if (positionNumber == 10)
                        {
                            freqsLast[chosenRank]++;
                        }

                        break;
                    }
                }

                sw.Write(line);
                sw.WriteLine("\t\t " + chosenRank);

                //Console.WriteLine(chosenRank);
                line = sr.ReadLine();
            }

            for (int j = 0; j < 21; j++)
            {
                probs[j] = probs[j] / (50.0 * 10);
                freqsFirst[j] = freqsFirst[j] / 50.0;
                freqsLast[j] = freqsLast[j] / 50.0;
            }

            sw.Close();
            fs2.Close();

            sr.Close();
            fs.Close();

           // Console.ReadLine();
        }
    }
}
