using RestaurantCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentsSqlBuilder
{
    class SqlBuilder
    {
        public static void Main(string[] args)
        {
            FileStream fs = new FileStream("Vectors.txt", FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            FileStream fs2 = new FileStream("SqlCommands.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs2);

            double[] changes = new double[Constants.TotalInvestmentsTurns];

            int vectorNumber = 0;

            sw.WriteLine("delete from Vectors;");
            sw.WriteLine();

            string line = sr.ReadLine();
            while (line != null)
            {
                if (line.StartsWith("--") || line == string.Empty)
                {
                    sw.WriteLine(line);

                    if (line.StartsWith("--"))
                    {
                        vectorNumber++;
                    }

                    line = sr.ReadLine();
                    continue;
                }

                string[] changesStr = line.Split(' ');

                int i = 0;
                foreach (string changeStr in changesStr)
                {
                    if (changeStr == string.Empty)
                    {
                        continue;
                    }

                    changes[i] = double.Parse(changeStr);
                    i++;
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("Insert Into Vectors(VectorNum");

                for (int j = 1; j <= Constants.TotalInvestmentsTurns; j++)
                {
                    sb.Append(", Turn" + j);
                }
                sb.Append(") Values (");

                sb.Append(vectorNumber);

                for (int j = 0; j < Constants.TotalInvestmentsTurns; j++)
                {
                    sb.Append(", " + changes[j]);
                }
                sb.Append(");");

                sw.WriteLine(sb.ToString());

                line = sr.ReadLine();
            }

            sw.Close();
            fs2.Close();

            sr.Close();
            fs.Close();

            // Console.ReadLine();
        }
    }
}
