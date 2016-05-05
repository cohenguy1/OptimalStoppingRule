using RestaurantCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RandomVectors
{
    public class Program
    {
        public const int NumOfVectors = 50;

        public const string VectorsFile = "Vectors2.txt";

        public static void Main(string[] args)
        {
            if (File.Exists(VectorsFile))
            {
                File.Delete(VectorsFile);
            }

            FileStream output = new FileStream(VectorsFile, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(output);

            var positionCandidates = Generation.GenerateCandidatesForPosition();

            for (var i = 0; i < NumOfVectors; i++)
            {
                sw.WriteLine("-- Vector " + (i + 1));
                for (var positionIndex = 0; positionIndex < 10; positionIndex++)
                {
                    Thread.Sleep(25);
                    Generation.InitCandidatesForPosition(positionCandidates);
                    WriteVector(positionCandidates, sw);
                    sw.WriteLine();
                }

                sw.WriteLine();
                sw.WriteLine();
            }

            sw.Close();
            output.Close();
        }

        private static void WriteVector(List<Candidate> positionCandidates, StreamWriter sw)
        {
            foreach (var candidate in positionCandidates)
            {
                sw.Write(candidate.CandidateRank + " ");
            }
        }
    }
}
