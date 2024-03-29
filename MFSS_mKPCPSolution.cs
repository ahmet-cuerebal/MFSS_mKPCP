using MFSS_mKPCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MFSS_mKPCP.MFSS_mKPCPSolution;

namespace MFSS_mKPCP
{
    internal class MFSS_mKPCPSolution
    {
        public enum MFSS_mKPCPItemStatus { Using, NotUsing, Unknown};
        MFSS_mKPCPInstance     mInstanace;
        MFSS_mKPCPItemStatus[] mItemStatuses;
        double              mObjective;
        public int       mNumCreatedDuplicate;

        double[]         mContributions;
        double         mAvaillableCapacity;   

        List<int>           mAvaillableItems;
        List<int>           mSelectedItems;
        
        Random mGenerator;


        public List<int> AvaillableItems
        { 
            get { return mAvaillableItems; }
        }

        public double[] Contributions
        {
            get { return mContributions; }
        }


        public void SetGenerator(Random iGenerator) { 
        
            mGenerator = iGenerator;
        }


        public void InitGreedy() {

            mContributions = new double[mInstanace.NumItems];
            mAvaillableItems = new List<int>();
            mSelectedItems = new List<int>();
            for (int i = 0; i < mInstanace.NumItems; i++) {

                mContributions[i] = mInstanace.ItemWeights[i] / mInstanace.ItemValues[i];
                mAvaillableItems.Add(i);
                mItemStatuses[i] = MFSS_mKPCPItemStatus.Unknown;
            }

            mAvaillableCapacity = -mInstanace.Capacity;    

        }

        public bool AddItem(int iIndex) {

            if (mItemStatuses[iIndex] == MFSS_mKPCPItemStatus.Using)
                return false;

            mItemStatuses[iIndex] = MFSS_mKPCPItemStatus.Using;
            mAvaillableCapacity += mInstanace.ItemWeights[iIndex];

            mAvaillableItems.Remove(iIndex);
            mSelectedItems.Add(iIndex);
            return true;
        }

        public MFSS_mKPCPItemStatus[] ItemStatuses
        {
            get { return mItemStatuses; }
        }

        public double Objective
        { 
            get { return mObjective; }
        }

        // to see how much space we need to fill up left 
        public void CalculateAvaillableCapacity() {



                mAvaillableCapacity = -mInstanace.Capacity;

            for (int i = 0; i < mInstanace.NumItems; i++)
            {
                if (mItemStatuses[i] == MFSS_mKPCPItemStatus.Using)
                {

                        mAvaillableCapacity += mInstanace.ItemWeights[i];
                }
            }

        }

        public bool CanSwap(int GoingOut, int GoingIn) {

            if (mAvaillableCapacity - mInstanace.ItemWeights[GoingOut] + mInstanace.ItemWeights[GoingIn] >= 0 )
                return false;

            return true;
        }

        public MFSS_mKPCPSolution(MFSS_mKPCPInstance iInstanace)
        {
            mInstanace = iInstanace;
            Allocate();
        }


        // whether there is an improve
        public bool Improve(Random iGenerator) {

            List<int> Indexes = new List<int>();

            for (int i = 0; i < mInstanace.NumItems; i++)
                Indexes.Add(i);

            MFSS_mKPCPProblem.shuffle<int>(Indexes, iGenerator);
            CalculateAvaillableCapacity();

            int GoingOut;
            int GoingIn;
            bool Improve = true ;

            while (Improve)
            {
                Improve = false;
                for (int i = 0; i < mInstanace.NumItems; i++)
                {
                    for (int j = i + 1; j < mInstanace.NumItems; j++)
                    {
                        if (ItemStatuses[Indexes[i]] != ItemStatuses[Indexes[j]])
                        {

                            if (ItemStatuses[Indexes[i]] == MFSS_mKPCPItemStatus.Using)
                            {
                                GoingOut = Indexes[i];
                                GoingIn = Indexes[j];
                            }
                            else
                            {
                                GoingIn = Indexes[i];
                                GoingOut = Indexes[j];
                            }

                            if (mInstanace.ItemValues[GoingIn] >= mInstanace.ItemValues[GoingOut])
                                continue;

                            if (CanSwap(GoingOut, GoingIn))
                            {
                                Improve = true;
                                ItemStatuses[GoingOut] = MFSS_mKPCPItemStatus.NotUsing;
                                ItemStatuses[GoingIn] = MFSS_mKPCPItemStatus.Using;

                                    mAvaillableCapacity += mInstanace.ItemWeights[GoingIn] - mInstanace.ItemWeights[GoingOut];

                            }
                        }
                    }
                }
            }

            return false;
        }

        public MFSS_mKPCPSolution(List<int> indexes, MFSS_mKPCPInstance iInstanace)
        {


            mInstanace = iInstanace;
            Allocate();

            for (int i = 0; i < mInstanace.NumItems; i++)
            {
                mItemStatuses[i] = MFSS_mKPCPItemStatus.NotUsing;
            }

            foreach (int index in indexes) {
                mItemStatuses[index] = MFSS_mKPCPItemStatus.Using;
            }
            mSelectedItems = indexes;
            
        }

        public void Allocate() {

            mItemStatuses = new MFSS_mKPCPItemStatus[mInstanace.NumItems];
            mSelectedItems = new List<int>();
        }

        public void ResetSolution() {

            for (int i = 0; i < mInstanace.NumItems; i++) {
                mItemStatuses[i] = MFSS_mKPCPItemStatus.Unknown;
            }
            mSelectedItems.Clear();
        }

        public int GetNumberOfUsedItems() {

            int Result = 0;

            for (int i = 0; i < mInstanace.NumItems; i++) {
                if (mItemStatuses[i] == MFSS_mKPCPItemStatus.Using)
                    Result++;
            }

            return Result;
        
        }

        public void SaveSolution(string FileName) {

            StreamWriter T = new StreamWriter(FileName);
                
            T.WriteLine(GetNumberOfUsedItems());

            for (int i = 0; i < mInstanace.NumItems; i++) {

                if (ItemStatuses[i] == MFSS_mKPCPItemStatus.Using)
                    T.WriteLine(i);
            }
        
            T.Close();
        }

        public void LoadSolution(string FileName)
        {

            string[] Lines = File.ReadAllLines(FileName);
            int line = 0;
            int NumSelected = Convert.ToInt16(Lines[line++]);
            int Index;

            for (int i = 0; i < mInstanace.NumItems; i++) {
                ItemStatuses[i] = MFSS_mKPCPSolution.MFSS_mKPCPItemStatus.NotUsing;
            }

            for (int i = 0; i < NumSelected; i++)
            {
                Index = Convert.ToInt16(Lines[line++]);
                ItemStatuses[Index] = MFSS_mKPCPSolution.MFSS_mKPCPItemStatus.NotUsing;
                    
            }

            
        }


        public void SetStatusItem(int ItemIndex, MFSS_mKPCPItemStatus iStatus) {
            mItemStatuses[ItemIndex] = iStatus;
        }

        public bool IsSame(MFSS_mKPCPSolution iSolution) {

            for (int i = 0; i < mInstanace.NumItems; i++) {

                if (ItemStatuses[i] != iSolution.ItemStatuses[i])
                    return false;
            }

            return true;
        }

        public double GetTotalWeight() {

            double Result = 0;

            for (int i = 0; i < mInstanace.NumItems; i++)
            {
                if (mItemStatuses[i] == MFSS_mKPCPItemStatus.Using)
                    Result += mInstanace.ItemWeights[i];
            }

            return Result;
        
        }


        public double CalculateObjective()
        {
            double sum = 0;

            for (int i = 0; i < mInstanace.NumItems; i++)
            {
                if (mItemStatuses[i] == MFSS_mKPCPItemStatus.Using)
                    sum += mInstanace.ItemValues[i];
            }
            //foreach (var penalty in mInstanace.Penalties)
            //    {
            //        if (mItemStatuses[penalty.itemIndex] == MFSS_mKPCPItemStatus.NotUsing)
            //        {
            //            sum += penalty.penalty;
            //            //Console.WriteLine("P " + penalty.itemIndex);
            //            //Console.WriteLine("P " + penalty.penalty);
            //        }
            //    }

            mObjective = sum; 
            return sum; 
        }


        public void AddInfoFromSolution(MFSS_mKPCPSolution iSolution, List<int> Translate) {

            for (int i = 0; i < iSolution.ItemStatuses.Length; i++) {

                mItemStatuses[Translate[i]] = iSolution.ItemStatuses[i];
            }
        }



    }
}
