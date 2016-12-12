using GamesCommon;
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
        public const string VectorsFile = "Vectors.txt";

        public const string VectorCommandsFile = "SqlCommands.txt";

        public const string ProbsCommandsFile = "SqlCommands2.txt";

        public static void Main(string[] args)
        {
            WriteVectorCommands();

            var changes = GetChanges();

            WriteProbsCommands(changes);

            // Console.ReadLine();
        }

        private static void WriteVectorCommands()
        {
            FileStream fs = new FileStream(VectorsFile, FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            FileStream fs2 = new FileStream(VectorCommandsFile, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs2);

            int[] changes = new int[Constants.TotalInvestmentsTurns];

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

                    changes[i] = int.Parse(changeStr);
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
        }

        private static void WriteProbsCommands(int[] changes)
        {
            FileStream fs2 = new FileStream(ProbsCommandsFile, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs2);

            string deleteCommand = ("delete from Changes;" + Environment.NewLine);
            sw.WriteLine(deleteCommand);

            foreach (var change in changes)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Insert Into Changes(Change) Values (");
                sb.Append(change);
                sb.Append(");");

                sw.WriteLine(sb.ToString());
            }

            sw.Close();
            fs2.Close();
        }

        private static int[] GetChanges()
        {
            FileStream fs = new FileStream("NasdaqChange.txt", FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            int[] changes = new int[Constants.NumOfChanges];

            for (int i = 0; i < Constants.NumOfChanges; i++)
            {
                string line = sr.ReadLine();

                changes[i] = int.Parse(line);
            }

            return changes;
        }
    }
}
