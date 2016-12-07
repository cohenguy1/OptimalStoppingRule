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

            int count = 0;
            string line = sr.ReadLine();
            while (!string.IsNullOrEmpty(line))
            {
                changes.Add(double.Parse(line));
                count++;
                line = sr.ReadLine();
            }

            double[] selectedChanges = new double[Constants.NumOfChanges];

            Random r = new Random();
            List<int> seenIndexes = new List<int>();

            count = 0;
            while (count < Constants.NumOfChanges)
            {
                var index = r.Next(800);

                if (seenIndexes.Contains(index))
                {
                    continue;
                }

                seenIndexes.Add(index);
                
                var currentNasdaqValue = changes[index];
                
                var lastYearValue = changes[index + r.Next(100) + 265];

                var diff = Math.Abs(currentNasdaqValue - lastYearValue) / currentNasdaqValue * 100;
                selectedChanges[count] = diff;
                count++;
            }

            if (File.Exists(ChangesOutputFile))
            {
                File.Delete(ChangesOutputFile);
            }

            FileStream fs2 = new FileStream(ChangesOutputFile, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs2);

            for (int i = 0; i < Constants.NumOfChanges; i++)
            {
                sw.WriteLine(selectedChanges[i]);
            }

            sw.Close();
            fs2.Close();
        }
    }
}
