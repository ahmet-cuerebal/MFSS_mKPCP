using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics.Metrics;
using System.Text.Json;
using static MFSS_mKPCP.MFSS_mKPCPSolution;

namespace MFSS_mKPCP
{
    internal class MFSS_mKPCPInstance
    {

        private int mNumItems;   

        private double[] mItemValues;   
        private double[] mItemWeights;   
        private double mCapacity;   
        private int mDelta;
        private int mOptimal;
        private string lastLoadedFileName;


        private List<(int itemIndex, int penalty)> mPenalties;

        public List<(int itemIndex, int penalty)> Penalties
        {
            get { return mPenalties; }
        }


        public int NumItems
        {
            get { return mNumItems; }
        }

        public int Delta
        {
            get { return mDelta; }
        }


        public int Optimal
        {
            get { return mOptimal; }
        }

        public double[] ItemValues
        {
            get { return mItemValues; }
        }

        public double Capacity
        {
            get { return mCapacity; }
        }
        public double[] ItemWeights
        {
            get { return mItemWeights; }
        }


        public MFSS_mKPCPInstance()
        {

        }

        public void LoadOptimum(string FileName, int Index)
        {
            string[] Lines = File.ReadAllLines(FileName);
            string[] words = Lines[Index].Split(',');
            double result;

            if (double.TryParse(words[1], out result))
            {
                mOptimal = (int)Math.Round(result);
            }
        }


        public MFSS_mKPCPInstance(int iNumItems)
        {

            mNumItems = iNumItems;
            Allocate();
        }

        public void Allocate()
        {

            mItemValues = new double[mNumItems];
            mItemWeights = new double[mNumItems];

        }


        public bool Load(string FileName)
        {
            string jsonString = File.ReadAllText(FileName);

            var jsonDoc = JsonDocument.Parse(jsonString);

            var root = jsonDoc.RootElement;

            mNumItems = root.GetProperty("n_items").GetInt32();
            mCapacity = root.GetProperty("min_weight").GetDouble(); 
            mDelta = root.GetProperty("max_distance").GetInt32();

            mItemValues = new double[mNumItems]; 
            mItemWeights = new double[mNumItems]; 

            var profits = root.GetProperty("profits").EnumerateArray();
            int index = 0;
            foreach (var profit in profits)
            {
                mItemValues[index++] = profit.GetDouble(); 
            }

            var weights = root.GetProperty("weights").EnumerateArray();
            index = 0;
            foreach (var weight in weights)
            {
                mItemWeights[index++] = weight.GetDouble(); 
            }
            lastLoadedFileName = FileName;
            return true;
        }

        public void ClearPenalties()
        {
            mPenalties.Clear();
        }

        public void AssignRandomPenaltiesAndSaveJson(int seed = 42)
        {
            Random rng = new Random(seed);
            mPenalties = new List<(int itemIndex, int penalty)>();
            string newBasePath = @"C:\Users\AhmetCRBL\Desktop\MFSS_mKPCP\instances";
            int numPenalties = (int)(mNumItems * 0.2);

            HashSet<int> penalizedItems = new HashSet<int>();
            while (penalizedItems.Count < numPenalties)
            {
                int itemIndex = rng.Next(0, mNumItems); 
                penalizedItems.Add(itemIndex);
            }

            foreach (int itemIndex in penalizedItems)
            {
                int penaltyValue = rng.Next(1, 11); 
                mPenalties.Add((itemIndex, penaltyValue)); 
            }
            string originalFileName = Path.GetFileName(lastLoadedFileName);
            var penaltiesList = mPenalties.Select(p => new { itemIndex = p.itemIndex, penalty = p.penalty }).ToList();

            string jsonString = File.ReadAllText(lastLoadedFileName);
            using var jsonDoc = JsonDocument.Parse(jsonString);
            var jsonObject = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonString);

            jsonObject["penalties"] = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(penaltiesList));

            string updatedJsonString = JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions { WriteIndented = true });

            string newFileName = Path.Combine(newBasePath, Path.GetFileNameWithoutExtension(originalFileName) + "_P" + Path.GetExtension(originalFileName));
            File.WriteAllText(newFileName, updatedJsonString);

        }


        public void ZeroPenalties()
        {
            for (int i = 0; i < mPenalties.Count; i++)
            {
                var penalty = mPenalties[i];
                mPenalties[i] = (penalty.itemIndex, 0);
            }
        }

    }

}


