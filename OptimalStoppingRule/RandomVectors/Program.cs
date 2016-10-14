using RestaurantCommon;
using System;
using System.Collections.Generic;
using System.IO;

namespace RandomVectors
{
    public class Program
    {
        public const int NumOfVectors = 50;

        public const string VectorsFile = "Vectors.txt";

        public static void Main(string[] args)
        {
            
            var positionCandidates = Generation.GenerateCandidatesForPosition();
            Random random = new Random();

            int[] acceptedCount = new int[10];

            while (acceptedCount[0] > 8 || acceptedCount[0] == 0)
            {
                for (int i = 0; i < acceptedCount.Length; i++)
                {
                    acceptedCount[i] = 0;
                }

                if (File.Exists(VectorsFile))
                {
                    File.Delete(VectorsFile);
                }

                FileStream output = new FileStream(VectorsFile, FileMode.CreateNew);
                StreamWriter sw = new StreamWriter(output);

                for (var i = 0; i < NumOfVectors; i++)
                {
                    sw.WriteLine("-- Vector " + (i + 1));
                    for (var positionIndex = 0; positionIndex < 10; positionIndex++)
                    {
                        Generation.InitCandidatesForPosition(positionCandidates, random);
                        if (positionIndex == 0)
                        {
                            var candidatesByNow = new List<Candidate>();
                            for (int candidateIndex = 0; candidateIndex < Constants.TotalCandidates; candidateIndex++)
                            {
                                var currentCandidate = positionCandidates[candidateIndex];
                                DecisionMaker.GetInstance().DetermineCandidateRank(candidatesByNow, currentCandidate);

                                if (currentCandidate.CandidateAccepted)
                                {
                                    acceptedCount[currentCandidate.CandidateRank - 1]++;
                                    break;
                                }
                            }
                        }
                        WriteVector(positionCandidates, sw);
                        sw.WriteLine();
                    }

                    sw.WriteLine();
                    sw.WriteLine();
                }

                sw.Close();
                output.Close();
            }

            Console.WriteLine("Completed!");
            Console.ReadLine();
            
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
