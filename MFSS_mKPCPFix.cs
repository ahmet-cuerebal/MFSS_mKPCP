using MFSS_mKPCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFSS_mKPCP

{
    internal class MFSS_mKPCPCplexEXT
    {
        public MFSS_mKPCPSolution.MFSS_mKPCPItemStatus[] mStates;
        public int                           mNumElements;
        public MFSS_mKPCPSolution                  mHotStart;



        public void InitAll() {

            mNumElements = -1;
            mStates = null;
            mHotStart = null;

        }
        public MFSS_mKPCPCplexEXT()
        {

            InitAll();
        }

        public MFSS_mKPCPCplexEXT(MFSS_mKPCPSolution.MFSS_mKPCPItemStatus[] iStates)
        {
            InitAll();
            mStates = iStates;
        }
        public MFSS_mKPCPCplexEXT(int iNumElements)
        {
            InitAll();
            mNumElements = iNumElements;
        }

        public MFSS_mKPCPCplexEXT(MFSS_mKPCPSolution iHotStart)
        {
            mNumElements = -1;
            mStates = null;
            mHotStart = iHotStart;
        }
    }
}
