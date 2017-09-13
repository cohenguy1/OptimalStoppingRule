using GamesCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlComparisonCommandGenerator
{
    public class SqlComparisonCommandGenerator
    {
        private const string InputFile = "VectorsOutput.txt";

        private const string OutputFile = "ComparisonCommand";

        private const int OptimalRatingColumn = 1;
        private const int MonteCarloRatingColumn = 2;
        
        public static void Main(string[] args)
        {
            var fs = new FileStream(InputFile, FileMode.Open);
            var sr = new StreamReader(fs);

            var commandMC = new StringBuilder();
            var commandOptimal = new StringBuilder();
            commandOptimal.Append("( ");
            commandMC.Append("( ");
            sr.ReadLine();
            for (var vectorNum = 1; vectorNum <= Constants.NumOfVectors; vectorNum++)
            {
                var line = sr.ReadLine();
                var splittedLine = line.Split('\t');

                var ratingPositionOptimal = splittedLine[OptimalRatingColumn];
                var ratingPositionMC = splittedLine[MonteCarloRatingColumn];

                commandOptimal.Append("((VectorNum = " + vectorNum + ") and (RatingPosition != " + ratingPositionOptimal + ")) ");
                commandMC.Append("((VectorNum = " + vectorNum + ") and (RatingPosition != " + ratingPositionMC + ")) ");
                if (vectorNum < Constants.NumOfVectors)
                {
                    commandOptimal.Append(" or ");
                    commandMC.Append(" or ");
                }
            }
            commandOptimal.Append(")");
            commandMC.Append(")");

            sr.Close();
            fs.Close();

            writeCommand(commandOptimal, "-Optimal.txt");
            writeCommand(commandMC, "-MonteCarlo.txt");
        }

        private static void writeCommand(StringBuilder command, string file)
        {
            var fsOutput = new FileStream(OutputFile + file, FileMode.Create);
            var sw = new StreamWriter(fsOutput);

            sw.WriteLine(command.ToString());

            sw.Close();
            fsOutput.Close();
        }
    }
}
