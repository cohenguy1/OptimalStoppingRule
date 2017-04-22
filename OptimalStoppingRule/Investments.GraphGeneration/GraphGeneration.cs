using GamesCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investments.GraphGeneration
{
    public class GraphGeneration
    {
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

            Dictionary<int, double> probs = new Dictionary<int, double>();

            for (int i = 0; i < Constants.InvestmentsNumOfChanges; i++)
            {
                if (!probs.ContainsKey(changes[i]))
                {
                    probs.Add(changes[i], 0);
                }
                probs[changes[i]] += 1.0/Constants.InvestmentsNumOfChanges;
            }

            var ordered = probs.OrderBy(x => x.Key);

            FileStream fs2 = new FileStream("Output.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs2);

            foreach (var item in ordered)
            {
                sw.Write("\t\t\t\t");
                sw.Write("{x: ");
                sw.Write(item.Key);
                sw.Write(", y: ");
                sw.Write(item.Value);
                sw.Write("}, " + Environment.NewLine);
            }

            sw.Write(Environment.NewLine);
            sw.Write(Environment.NewLine);
            sw.Write(Environment.NewLine);

            sw.Close();
            sr.Close();
            fs.Close();
            fs2.Close();
        }
    }
}
