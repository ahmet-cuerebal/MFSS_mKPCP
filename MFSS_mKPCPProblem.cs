using ILOG.CPLEX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFSS_mKPCP
{

    
    internal class MFSS_mKPCPProblem
    {

        MFSS_mKPCPInstance mInstance;
        Random mGenerator;
        MFSS_mKPCPSolution mSolution;
        MFSS_mKPCPSolution mBestSolution;
        MFSS_mKPCPSolutionTracker mSolutionTracker;


        List<double> mIntermediateSolutions;
        List<long> mIntermediateSolutionsTimes;
        List<long> mIntermediateSolutionsIterations;
        Stopwatch mStopWatch;
        int mNumberOfSolutionsGenerated;
        long mTimeLimit;
        int mRCLSize;
        RCL<int> mRCL;

        public long TimeLimit
        {
            get { return mTimeLimit;}
            set { mTimeLimit = value; }
        }

        public int RCLSize
        {
            get { return mRCLSize; }
            set { mRCLSize = value; mRCL = new RCL<int>(mRCLSize); }
        }

        public MFSS_mKPCPSolution Solution
        {
            get { return mSolution; }
        }

        public MFSS_mKPCPSolution BestSolution
        {
            get { return mBestSolution; }
        }

        public void InitTracking()
        {

            mIntermediateSolutions = new List<double>();
            mIntermediateSolutionsTimes = new List<long>();
            mIntermediateSolutionsIterations = new List<long>();

        }

        public void SaveIntermediate(string Filename) {

            StreamWriter File = new StreamWriter(Filename);

            for (int i = 0; i < mIntermediateSolutions.Count; i++) {

                File.WriteLine(mIntermediateSolutions[i] + " - " + mIntermediateSolutionsIterations[i]+" - " + mIntermediateSolutionsTimes[i]);
            }
            File.Close();
        }

        public bool CheckBest() {

            if (mBestSolution == null) {
                Solution.CalculateObjective();
                mBestSolution = Solution;

                mIntermediateSolutions.Add(mBestSolution.CalculateObjective());
                mIntermediateSolutionsIterations.Add(mNumberOfSolutionsGenerated);
                mIntermediateSolutionsTimes.Add(mStopWatch.ElapsedMilliseconds);

                return true;
            }
            if (mBestSolution.CalculateObjective() > mSolution.CalculateObjective()) {
                mBestSolution = Solution;

                mIntermediateSolutions.Add(mBestSolution.CalculateObjective());
                mIntermediateSolutionsIterations.Add(mNumberOfSolutionsGenerated);
                mIntermediateSolutionsTimes.Add(mStopWatch.ElapsedMilliseconds);

                return true; 
            }

            return false;
        } 

        public MFSS_mKPCPProblem() {


        }
        public void InitRandom(int seed) {

            mGenerator = new Random(seed);
        }
        public MFSS_mKPCPProblem(MFSS_mKPCPInstance iInstance)
        {
            mInstance = iInstance;
            InitRandom(0);

        }

        public void SolPop(int mode)
        {
            List<int> selectedItems = new List<int>();
            double fillUp = mInstance.Capacity;

            if (mode == 0)
            {
                int i = 0;
                while (i <= mInstance.NumItems - mInstance.Delta)
                {
                    List<int> subsetIndices = Enumerable.Range(i, mInstance.Delta)
                                                        .Where(index => !selectedItems.Contains(index))
                                                        .ToList();

                    if (subsetIndices.Count > 0)
                    {

                        if (subsetIndices.Count == mInstance.Delta)
                        {
                            int itemsToSelect = mGenerator.Next(1, subsetIndices.Count + 1);
                            var shuffledSubsetIndices = subsetIndices.OrderBy(x => mGenerator.Next()).ToList();
                            for (int idx = 0; idx < itemsToSelect; idx++)
                            {
                                int selectedIndex = shuffledSubsetIndices[idx];
                                selectedItems.Add(selectedIndex);
                                fillUp -= mInstance.ItemWeights[selectedIndex];
                                if (fillUp <= 0) break;
                            }
                        }
                        else
                        {
                            var shuffledSubsetIndices = subsetIndices.OrderBy(x => mGenerator.Next()).ToList();
                            int itemsToSelect = mGenerator.Next(0, subsetIndices.Count + 1);
                            for (int idx = 0; idx < itemsToSelect; idx++)
                            {
                                int selectedIndex = shuffledSubsetIndices[idx];
                                selectedItems.Add(selectedIndex);
                                fillUp -= mInstance.ItemWeights[selectedIndex];
                                if (fillUp <= 0) break;
                            }
                        }
                    }
                    if (fillUp <= 0) break;
                    i += 1;
                }
            }
            else if (mode == 1)
            {
                int i = mInstance.NumItems - mInstance.Delta;

                while (i >= 0 + mInstance.Delta - 1)
                {
                    List<int> subsetIndices = Enumerable.Range(i, mInstance.Delta)
                                                        .Where(index => !selectedItems.Contains(index))
                                                        .Reverse().ToList();

                    if (subsetIndices.Count > 0)
                    {

                        if (subsetIndices.Count == mInstance.Delta)
                        {
                            int itemsToSelect = mGenerator.Next(1, subsetIndices.Count + 1);
                            var shuffledSubsetIndices = subsetIndices.OrderBy(x => mGenerator.Next()).ToList();
                            for (int idx = 0; idx < itemsToSelect; idx++)
                            {
                                int selectedIndex = shuffledSubsetIndices[idx];
                                selectedItems.Add(selectedIndex);
                                fillUp -= mInstance.ItemWeights[selectedIndex];
                                if (fillUp <= 0) break;
                            }
                        }
                        else
                        {
                            var shuffledSubsetIndices = subsetIndices.OrderBy(x => mGenerator.Next()).ToList();
                            int itemsToSelect = mGenerator.Next(0, subsetIndices.Count + 1);
                            for (int idx = 0; idx < itemsToSelect; idx++)
                            {
                                int selectedIndex = shuffledSubsetIndices[idx];
                                selectedItems.Add(selectedIndex);
                                fillUp -= mInstance.ItemWeights[selectedIndex];
                                if (fillUp <= 0) break;
                            }
                        }
                    }
                    if (fillUp <= 0) break;
                    i -= 1;
                }


            }
            else if (mode == 2)
            {
                int middleIndex = mInstance.NumItems / 2; // Determine middle index
                int left = middleIndex - 1;
                int right = middleIndex;
                bool chooseRight = true; // Start by choosing right

                while ((left >= 0 || right < mInstance.NumItems) && fillUp > 0)
                {
                    List<int> subsetIndices;
                    if (chooseRight && right < mInstance.NumItems)
                    {
                        subsetIndices = Enumerable.Range(right, Math.Min(mInstance.Delta, mInstance.NumItems - right))
                                                  .Where(index => !selectedItems.Contains(index))
                                                  .ToList();
                        right += 1; // Prepare for next right selection
                    }
                    else if (!chooseRight && left >= 0)
                    {
                        int rangeStart = Math.Max(0, left - mInstance.Delta + 1);
                        subsetIndices = Enumerable.Range(rangeStart, Math.Min(left + 1 - rangeStart, mInstance.Delta))
                                                  .Where(index => !selectedItems.Contains(index))
                                                  .ToList();
                        left -= 1; // Prepare for next left selection
                    }
                    else
                    {
                        break; // Exit if neither side has valid options
                    }

                    if (subsetIndices.Count > 0)
                    {

                        if (subsetIndices.Count == mInstance.Delta)
                        {
                            int itemsToSelect = mGenerator.Next(1, subsetIndices.Count+1);
                            var shuffledSubsetIndices = subsetIndices.OrderBy(x => mGenerator.Next()).ToList();
                            for (int idx = 0; idx < itemsToSelect; idx++)
                            {
                                int selectedIndex = shuffledSubsetIndices[idx];
                                selectedItems.Add(selectedIndex);
                                fillUp -= mInstance.ItemWeights[selectedIndex];
                                if (fillUp <= 0) break;
                            }
                        }
                        else
                        {
                            var shuffledSubsetIndices = subsetIndices.OrderBy(x => mGenerator.Next()).ToList();
                            int itemsToSelect = mGenerator.Next(0, subsetIndices.Count+1);
                            for (int idx = 0; idx < itemsToSelect; idx++)
                            {
                                int selectedIndex = shuffledSubsetIndices[idx];
                                selectedItems.Add(selectedIndex);
                                fillUp -= mInstance.ItemWeights[selectedIndex];
                                if (fillUp <= 0) break;
                            }
                        }
                    }
                    if (fillUp <= 0) break;
                    chooseRight = !chooseRight; 
                }
            }

            else
            {
                var sortedItems = mInstance.ItemWeights
                    .Select((weight, index) => new { Index = index, Weight = weight })
                    .OrderByDescending(item => item.Weight)
                    .ToList();

                int startIndex = mGenerator.Next(0, Math.Min(5, sortedItems.Count));
                var startingItem = sortedItems[startIndex];
                selectedItems.Add(startingItem.Index);
                fillUp -= mInstance.ItemWeights[startingItem.Index];

                int left = startingItem.Index - 1;
                int right = startingItem.Index + 1;
                bool chooseRight = true; 

                while (fillUp > 0)
                {
                    List<int> subsetIndices = new List<int>();
                    bool selectedThisRound = false;

                    if (chooseRight && right < mInstance.NumItems)
                    {
                        subsetIndices = Enumerable.Range(right, Math.Min(mInstance.Delta, mInstance.NumItems - right))
                                                  .Where(index => !selectedItems.Contains(index))
                                                  .ToList();
                        right += 1; 
                    }
                    else if (!chooseRight && left >= 0)
                    {
                        int rangeStart = Math.Max(0, left - mInstance.Delta + 1);
                        subsetIndices = Enumerable.Range(rangeStart, Math.Min(left + 1 - rangeStart, mInstance.Delta))
                                                  .Where(index => !selectedItems.Contains(index))
                                                  .ToList();
                        left -= 1; 
                    }

                    if (subsetIndices.Count > 0)
                    {

                        if (subsetIndices.Count == mInstance.Delta)
                        {
                            int itemsToSelect = mGenerator.Next(1, subsetIndices.Count + 1);
                            var shuffledSubsetIndices = subsetIndices.OrderBy(x => mGenerator.Next()).ToList();
                            for (int idx = 0; idx < itemsToSelect; idx++)
                            {
                                int selectedIndex = shuffledSubsetIndices[idx];
                                selectedItems.Add(selectedIndex);
                                fillUp -= mInstance.ItemWeights[selectedIndex];
                                if (fillUp <= 0) break;
                            }
                        }
                        else
                        {
                            var shuffledSubsetIndices = subsetIndices.OrderBy(x => mGenerator.Next()).ToList();
                            int itemsToSelect = mGenerator.Next(0, subsetIndices.Count + 1);
                            for (int idx = 0; idx < itemsToSelect; idx++)
                            {
                                int selectedIndex = shuffledSubsetIndices[idx];
                                selectedItems.Add(selectedIndex);
                                fillUp -= mInstance.ItemWeights[selectedIndex];
                                if (fillUp <= 0) break;
                            }
                        }
                    }

                    if (!selectedThisRound && left < 0 && right >= mInstance.NumItems) break;

                    if (left < 0) chooseRight = true;
                    else if (right >= mInstance.NumItems) chooseRight = false;
                    else chooseRight = !chooseRight; 
                }
            }


            if (fillUp > 0)
            {
                List<int> unselectedItems = Enumerable.Range(0, mInstance.NumItems)
                                                      .Where(index => !selectedItems.Contains(index))
                                                      .ToList();


                while (fillUp > 0)
                {
                    List<int> validUnselectedItems = unselectedItems.Where(index =>
                        selectedItems.Any(selectedIndex => Math.Abs(index - selectedIndex) <= mInstance.Delta))
                        .ToList();

                    if (!validUnselectedItems.Any())
                    {
                        break; 
                    }

                    int randomIndex = validUnselectedItems[mGenerator.Next(validUnselectedItems.Count)]; 
                    selectedItems.Add(randomIndex); 
                    fillUp -= mInstance.ItemWeights[randomIndex]; 

                    unselectedItems.Remove(randomIndex); 
                }
            }


            mSolution = new MFSS_mKPCPSolution(selectedItems, mInstance);
        }


        public void SolveFixSet(int PopulationSize, int K, int MaxIterations, int iMaxBinVar, int MaxStag, double MaxCalcTime) {

            List<int> SolutionIndexes = new List<int>();
            List<int> SelIndexes = new List<int>();
            int BaseIndex;
            int cK;
            MFSS_mKPCPSolution BaseSolution;
            MFSS_mKPCPSolution.MFSS_mKPCPItemStatus[] Fix;
            MFSS_mKPCPCplexEXT cFix = new MFSS_mKPCPCplexEXT();
            MFSS_mKPCPCplex Solver;
            int Stag = 0;
            double cMaxCalcTime = MaxCalcTime;
            double BestMaxCalcTime = MaxCalcTime;

            int AddIndex;

            mStopWatch = new Stopwatch();
            mStopWatch.Start();

            InitTracking();
            mSolutionTracker = new MFSS_mKPCPSolutionTracker(PopulationSize, mInstance);

            for (int i = 0; i < PopulationSize; i++) {

                Console.WriteLine("Solution  " + i);
                SolPop(i % 4);
                CheckBest();
                mSolutionTracker.AddSolution(mSolution);

            }

            int cFixSize = 100;
            while (mNumberOfSolutionsGenerated < MaxIterations) {

                
                SolutionIndexes.Clear();
                for (int j = 0; j < mSolutionTracker.GetNumSolutions(); j++) {
                    SolutionIndexes.Add(j);
                }

                //Console.WriteLine("START +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                //for (int j = 0; j < SolutionIndexes.Count; j++)
                //{
                //    Console.WriteLine(j);
                //}

                BaseIndex = mGenerator.Next() % mSolutionTracker.GetNumSolutions();
                BaseSolution = mSolutionTracker.GetSolution(BaseIndex);
                //Console.WriteLine("BASESOL");
                BaseSolution.CalculateObjective();

                shuffle<int>(SolutionIndexes, mGenerator);
                SelIndexes.Clear();


                cK = 5 + mGenerator.Next() % K;

                if (mSolutionTracker.NumberOfSolutions < cK)
                    cK = mSolutionTracker.NumberOfSolutions;
                else
                    cK = K;
                
                
                for (int s = 0; s < cK; s++) {
                    SelIndexes.Add(SolutionIndexes[s]);
                
                }


                 Fix = mSolutionTracker.GetFix(BaseIndex, SelIndexes,  cFixSize, mGenerator);


                Solver = new MFSS_mKPCPCplex(mInstance);
                Solver.TimeLimit = cMaxCalcTime;

                cFix.mStates = Fix;
                cFix.mHotStart = mSolutionTracker.GetBestFit(Fix);
                Solver.Solve(cFix);


                mNumberOfSolutionsGenerated++;
                Stag++;


                if (Solver.Solution == null)
                    continue;

                mSolution = Solver.Solution;
                mSolution.CalculateObjective();
                AddIndex = mSolutionTracker.AddSolution(mSolution);

                if (CheckBest()) {

                    BestMaxCalcTime = cMaxCalcTime;
                }


                if (AddIndex >= 0 )
                {
                    Stag=0;
                }

                if (Stag >=  MaxStag)
                {
                       cMaxCalcTime = cMaxCalcTime * 2;

         //           if (BestMaxCalcTime / cMaxCalcTime > 4)
//                        break;
                       Stag = 0;
                }


                if (mBestSolution.Objective == mInstance.Optimal)
                    break;
                if (mStopWatch.ElapsedMilliseconds > mTimeLimit)
                    break;
                if (cMaxCalcTime > 7)
                    break;
            }
        
        }
      
        static public void shuffle<T>(List<T> list, Random nGenerator)
        {
            //            Random rng = new Random();   // i.e., java.util.Random.
            int n = list.Count;        // The number of items left to shuffle (loop invariant).
            while (n > 1)
            {
                int k = nGenerator.Next(n);  // 0 <= k < n.
                n--;                     // n is now the last pertinent index;
                T temp = list[n];     // swap array[n] with array[k] (does nothing if k == n).
                list[n] = list[k];
                list[k] = temp;
            }
        }

    }

}
