using GamesCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InvestmentsChangesGenerator
{
    class ChangesGenerator
    {
        public const string TenYearsNasdaqFile = "NasdaqChange10Years.txt";

        public const string ChangesOutputFile = "NasdaqChange.txt";

        public static void Main(string[] args)
        {
            FileStream fs = new FileStream(TenYearsNasdaqFile, FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            List<double> changes = new List<double>();

            int totalChanges = 0;
            string line = sr.ReadLine();
            while (!string.IsNullOrEmpty(line))
            {
                changes.Add(double.Parse(line));
                totalChanges++;
                line = sr.ReadLine();
            }

            double[] selectedChanges = new double[Constants.NumOfChanges];

            Random r = new Random();
            List<int> seenIndexes = new List<int>();

            double minValue = 0;
            double maxValue = 0;

            while (selectedChanges.ToList().Count(x => x < 0) < 80)
            {
                var count = 0;
                selectedChanges = new double[Constants.NumOfChanges];
                seenIndexes = new List<int>();
                while (count < Constants.NumOfChanges)
                {
                    var index = r.Next(totalChanges);

                    seenIndexes.Add(index);

                    var currentNasdaqValue = changes[index];

                    var randomValue = changes[(index + r.Next(totalChanges)) % totalChanges];

                    var diff = (currentNasdaqValue - randomValue) / currentNasdaqValue * 100;
                    var absDiff = Math.Abs(diff);
                    
                    if (diff < 0)
                    {
                        diff *= 1.0 / 5;
                    }

                    if (diff < -70)
                    {
                        /*var rand = r.Next(3);
                        Thread.Sleep(50);
                        if (rand == 0)
                        {
                            diff = Math.Abs(diff);
                        }*/
                    }

                    selectedChanges[count] = diff;
                    count++;
                }

                minValue = selectedChanges.ToList().Min();
                maxValue = selectedChanges.ToList().Max();

                for (int i = 0; i < Constants.NumOfChanges; i++)
                {
                    selectedChanges[i] = (selectedChanges[i] - minValue) * 130 / (maxValue - minValue) - 30;
                }
            }
            
            if (File.Exists(ChangesOutputFile))
            {
                File.Delete(ChangesOutputFile);
            }

            FileStream fs2 = new FileStream(ChangesOutputFile, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs2);

            for (int i = 0; i < Constants.NumOfChanges; i++)
            {
                sw.WriteLine((int)selectedChanges[i]);
            }

            Console.WriteLine("Min: " + minValue);
            Console.WriteLine("Max: " + maxValue);

            Console.ReadLine();

            sw.Close();
            fs2.Close();
        }
    }
}
