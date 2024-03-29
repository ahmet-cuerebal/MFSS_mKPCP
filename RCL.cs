using System.Collections.Generic;
using System.Linq;

namespace MFSS_mKPCP
{
    class RCL<T>
    {

        double[] mValues;
        T[] mCandidates;
        int mSize;
        int mCurrentSize;

        double mMinValue;
        int mMinIndex;

        double mMaxValue;
        int mMaxIndex;


        public int CurrentSize
        {

            get { return mCurrentSize; }
        }

        public T GetCandidate(int Index) { return mCandidates[Index]; }
        public double GetValue(int Index) { return mValues[Index]; }

        public double MinValue
        {
            get { return mMinValue; }
        }
        public double MaxValue
        {
            get { return mMaxValue; }
        }

        public List<T> Sort()
        {

            List<double[]> TempList = new List<double[]>();
            double[] Temp;

            for (int i = 0; i < mCurrentSize; i++)
            {

                Temp = new double[2];

                Temp[0] = mValues[i];
                Temp[1] = i;

                TempList.Add(Temp);

            }

            TempList.OrderByDescending(o => o[0]);

            List<T> Result = new List<T>();

            foreach (double[] el in TempList)
            {

                Result.Add(mCandidates[(int)el[1]]);
            }

            return Result;

        }
        public RCL(int iSize)
        {

            mSize = iSize;

            mValues = new double[mSize];
            mCandidates = new T[mSize];
        }

        public void Reset()
        {

            mCurrentSize = 0;
            mMinValue = int.MaxValue;
            mMaxValue = int.MinValue;

        }

        public void Add(T iCandidate, double iValue)
        {


            if (mCurrentSize < mSize)
            {

                mValues[mCurrentSize] = iValue;
                mCandidates[mCurrentSize] = iCandidate;



                if (mMinValue > iValue)
                {
                    mMinValue = iValue;
                    mMinIndex = mCurrentSize;
                }

                if (mMaxValue < iValue)
                {
                    mMaxValue = iValue;
                    mMaxIndex = mCurrentSize;
                }


                mCurrentSize++;
                return;
            }

            if (mMinValue >= iValue)
                return;

            if (mMaxValue < iValue)
            {
                mMaxValue = iValue;
            }



            mValues[mMinIndex] = iValue;
            mCandidates[mMinIndex] = iCandidate;


            mMinIndex = -1;
            mMinValue = double.MaxValue;

            for (int i = 0; i < mSize; i++)
            {
                if (mMinValue > mValues[i])
                {

                    mMinValue = mValues[i];
                    mMinIndex = i;
                }

            }



        }


    }
}

