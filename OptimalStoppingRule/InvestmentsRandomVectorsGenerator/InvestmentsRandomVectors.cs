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

            double[] changes = new double[Constants.NumOfChanges];

            for (int i = 0; i < Constants.NumOfChanges; i++)
            {
                string line = sr.ReadLine();

                changes[i] = double.Parse(line);
            }

            GenerateRandomVectors(changes);

            sr.Close();
            fs.Close();
        }

        public static void GenerateRandomVectors(double[] changes)
        {
            if (File.Exists(VectorsFile))
            {
                File.Delete(VectorsFile);
            }

            FileStream output = new FileStream(VectorsFile, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(output);

            Random random = new Random();

            for (var i = 0; i < Constants.NumOfVectors; i++)
            {
                var changesList = changes.ToList();

                Console.WriteLine("Generating Vector " + (i + 1));

                sw.WriteLine("-- Vector " + (i + 1));

                for (int j = 0; j < Constants.TotalInvestmentsTurns; j++)
                {
                    var itemToRemove = random.Next(changesList.Count);

                    Thread.Sleep(40);

                    var change = changesList[itemToRemove];

                    sw.Write(change.ToString() + " ");

                    changesList.RemoveAt(itemToRemove);
                }

                sw.WriteLine();
                sw.WriteLine();
            }

            sw.Close();
            output.Close();
        }
    }
}
