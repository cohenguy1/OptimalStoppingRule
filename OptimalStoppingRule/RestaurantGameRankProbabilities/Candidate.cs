using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OptimalStoppingRule
{
    public class Candidate
    {
        public CandidateState CandidateState;

        public int CandidateRank;

        public int CandidateNumber;

        public bool CandidateAccepted;
    }
}