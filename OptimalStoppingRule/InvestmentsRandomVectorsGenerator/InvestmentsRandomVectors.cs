using GamesCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InvestmentsRandomVectorsGenerator
{
    public class InvestmentsRandomVectors
    {
        public const string VectorsFile = "Vectors.txt";

        public static void Main(string[] args)
        {
            FileStream fs = new FileStream("NasdaqChange.txt", FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            int[] changes = new int[Constants.InvestmentsNumOfChanges];

            for (int i = 0; i < Constants.InvestmentsNumOfChanges; i++)
            {
                string line = sr.ReadLine();

                changes[i] = int.Parse(line);
            }

            GenerateRandomVectors(changes);

            sr.Close();
            fs.Close();
        }

        public static void GenerateRandomVectors(int[] changes)
        {
            
            Random random = new Random();

            List<int[]> changesByPosition = new List<int[]>();
            for (int i = 0; i < Constants.TotalInvestmentsTurns; i++)
            {
                changesByPosition.Add(new int[Constants.NumOfVectors]);
            }

            var count = 0;
            var goodVectors = false;

            double[] ttests;

            while (!goodVectors)
            {
                if (File.Exists(VectorsFile))
                {
                    File.Delete(VectorsFile);
                }

                FileStream output = new FileStream(VectorsFile, FileMode.CreateNew);
                StreamWriter sw = new StreamWriter(output);
                for (var i = 0; i < Constants.NumOfVectors; i++)
                {
                    var changesList = changes.ToList();

                    Console.WriteLine("Generating Vector " + (i + 1));

                    sw.WriteLine("-- Vector " + (i + 1));

                    for (int j = 0; j < Constants.TotalInvestmentsTurns; j++)
                    {
                        var itemToRemove = random.Next(changesList.Count);

                        var change = changesList[itemToRemove];

                        changesByPosition[j][i] = change;

                        sw.Write(change.ToString() + " ");

                        changesList.RemoveAt(itemToRemove);
                    }

                    sw.WriteLine();
                    sw.WriteLine();
                }

                ttests = new double[Constants.TotalInvestmentsTurns];

                for (int i = 0; i < Constants.TotalInvestmentsTurns; i++)
                {
                    var f = changesByPosition[0].ToList().Select(x => (double)x).ToArray();
                    var s = changesByPosition[i].ToList().Select(x => (double)x).ToArray();

                    ttests[i] = Statistics.TTest(f, s);
                }

                goodVectors = true;
                for (int i = 1; i < Constants.TotalInvestmentsTurns; i++)
                {
                    if (ttests[i] < 0.25)
                    {
                        goodVectors = false;
                    }
                }

                sw.Close();
                output.Close();
            }

            var avgs = new double[Constants.TotalInvestmentsTurns];
            for (int i = 0; i < Constants.TotalInvestmentsTurns; i++)
            {
                avgs[i] = changesByPosition[i].Average(x => x);
            }
        }
    }
}
