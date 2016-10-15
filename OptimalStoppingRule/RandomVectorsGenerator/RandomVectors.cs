﻿using RestaurantCommon;
using System;
using System.Collections.Generic;
using System.IO;

namespace RandomVectorsGenerator
{
    public class RandomVectors
    {
        public const string VectorsFile = "Vectors.txt";

        public static void Main(string[] args)
        {

            var positionCandidates = Generation.GenerateCandidatesForPosition();
            Random random = new Random();
            Random rand2 = new Random();

            int[] acceptedCountOnFirst = new int[RestaurantConstants.TotalCandidates];
            int[] acceptedCountOnSecond = new int[RestaurantConstants.TotalCandidates];

            while (acceptedCountOnFirst[0] > 8 || acceptedCountOnFirst[0] == 0 ||
                   acceptedCountOnSecond[0] > 8)
            {
                for (int i = 0; i < acceptedCountOnFirst.Length; i++)
                {
                    acceptedCountOnFirst[i] = 0;
                    acceptedCountOnSecond[i] = 0;
                }

                if (File.Exists(VectorsFile))
                {
                    File.Delete(VectorsFile);
                }

                FileStream output = new FileStream(VectorsFile, FileMode.CreateNew);
                StreamWriter sw = new StreamWriter(output);

                for (var i = 0; i < RestaurantConstants.NumOfVectors; i++)
                {
                    sw.WriteLine("-- Vector " + (i + 1));
                    for (var positionIndex = 0; positionIndex < 10; positionIndex++)
                    {
                        Generation.InitCandidatesForPosition(positionCandidates, random);
                        if (positionIndex == 0)
                        {
                            IncreaseAcceptedCount(acceptedCountOnFirst, positionCandidates, rand2);
                        }
                        else if (positionIndex == 1)
                        {
                            IncreaseAcceptedCount(acceptedCountOnSecond, positionCandidates, rand2);
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

        private static void IncreaseAcceptedCount(int[] acceptedCount, List<Candidate> positionCandidates, Random rand)
        {
            var candidatesByNow = new List<Candidate>();
            for (int candidateIndex = 0; candidateIndex < RestaurantConstants.TotalCandidates; candidateIndex++)
            {
                var currentCandidate = positionCandidates[candidateIndex];
                DecisionMaker.GetInstance().DetermineCandidateRank(candidatesByNow, currentCandidate, rand);

                if (currentCandidate.CandidateAccepted)
                {
                    acceptedCount[currentCandidate.CandidateRank - 1]++;
                    break;
                }
            }
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