using OptimalStoppingRule;
using RestaurantCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantVectorFileReader
{
    class Program
    {
        public static void Main(string[] args)
        {
            FileStream fs = new FileStream("Vectors.txt", FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            FileStream fs2 = new FileStream("VectorsDecisions.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs2);

            int[] intRanks = new int[Constants.TotalCandidates];

            double[] probs = new double[Constants.TotalCandidates + 1];

            double[] freqsFirst = new double[Constants.TotalCandidates + 1];
            double[] freqsLast = new double[Constants.TotalCandidates + 1];

            int vectorNum = 0;

            int positionNumber = 0;

            string line = sr.ReadLine() ;
            while (line != null)
            {
                if (line.StartsWith("--") || line == string.Empty)
                {
                    if (line.StartsWith("--"))
                    {
                        vectorNum++;
                    }

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
                    DecisionMaker.GetInstance().DetermineCandidateRank(candidatesByNow, newCandidate);

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

            for (int j = 0; j < Constants.TotalCandidates + 1; j++)
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
