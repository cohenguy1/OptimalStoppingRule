using GamesCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSql
{
    class Program
    {
        public static void Main(string[] args)
        {
            FileStream fs = new FileStream("Vectors.txt", FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            FileStream fs2 = new FileStream("SqlCommands.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs2);

            int[] intRanks = new int[Constants.RestaurantNumOfCandidates];

            int vectorNumber = 0;
            int positionNumber = 0;

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
                        positionNumber = 0;
                        vectorNumber++;
                    }

                    line = sr.ReadLine();
                    continue;
                }

                string[] ranks = line.Split(' ');

                int i = 0;
                foreach (string rank in ranks)
                {
                    if (rank == string.Empty)
                    {
                        continue;
                    }

                    intRanks[i] = int.Parse(rank);
                    i++;
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("Insert Into Vectors(VectorNum, PositionNum, Rank1, Rank2, Rank3, Rank4, Rank5, Rank6, " +
                    "Rank7, Rank8, Rank9, Rank10) Values (");
                sb.Append(vectorNumber + ", ");
                sb.Append(positionNumber + ", ");

                for (int j = 0; j < intRanks.Length - 1; j++)
                {
                    sb.Append(intRanks[j] + ", ");
                }
                sb.Append(intRanks[intRanks.Length - 1] + ");");

                sw.WriteLine(sb.ToString());

                positionNumber++;

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
