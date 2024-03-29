using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.IO;

namespace MFSS_mKPCP
{
    internal class MFSS_mKPCPExperiments
    {
        string   mInstancesDirectory;
        string   mOutputDirectory;
        List<string> mProbleTypes;
        List<int> mProbleSizes;
        int      mNumInstances;
            

        public MFSS_mKPCPExperiments() {
        
        
        }

        void InitFiles() {
             mInstancesDirectory = "c:\\\\MFSS_mKPCP\\\\Instances\\\\";
             mOutputDirectory = "c:\\\\MFSS_mKPCP\\\\Instances\\\\Results\\\\";
             mProbleSizes = new List<int>();

            mProbleSizes.Add(200);
            mProbleSizes.Add(400);
            mProbleSizes.Add(600);

            mProbleTypes = new List<string>();
            mProbleTypes.Add("Constant");
            mProbleTypes.Add("OnePeak");
            mProbleTypes.Add("TwoPeak");

        }

        
        public void SolveAll() {

            MFSS_mKPCPProblem TestProblem;
            MFSS_mKPCPInstance TestInstance = new MFSS_mKPCPInstance();

            int             PopupationSize        = 200;
            int             MaxBinVar              =1000;
            double          LimitPerFix           =  15;
            int             MaxGeneratedSolutions = 500000;
            int             MaxStag = 50;
            long            TimeLimit             = 300;
            int             K = 5;


            
            InitFiles();
            string cResDirectory;
            string configString = "Res_P" + PopupationSize + "_MI" + MaxBinVar + "_TLF" + LimitPerFix + "_K" + K+"_M"+ MaxStag;
            string OutFile;
            string tFileName;
            int s = 1;
            string cDirectory;
            string[] filePaths;


            foreach (int size in mProbleSizes)
            {
                foreach (string ptype in mProbleTypes)
                {


                    cResDirectory = mOutputDirectory + configString + "\\\\";
                    if (!System.IO.Directory.Exists(cResDirectory))
                    {
                        System.IO.Directory.CreateDirectory(cResDirectory);
                    }

                    cDirectory = mInstancesDirectory + ptype + "\\\\" + size + "\\\\";
                    filePaths = Directory.GetFiles(cDirectory);

                    for (int fi = 0; fi < filePaths.Length; fi++)
                    {       

                        TestInstance = new MFSS_mKPCPInstance();
                        TestInstance.Load(filePaths[fi]);
                        TestInstance.AssignRandomPenaltiesAndSaveJson();
                        TestInstance.ZeroPenalties();
                        TestProblem = new MFSS_mKPCPProblem(TestInstance);
                        TestProblem.InitRandom(s);
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePaths[fi]);
                        //   OutFile = mInstancesDirectory+"Results\\\\" + "Res_" + ptype + "_" + size + "_"+fi+".txt" ;
                        OutFile = cResDirectory + "Res_" + ptype + "_" + size + "_" + fi + "_" + s + "_" + fileNameWithoutExtension + ".txt";
                        // TestInstance.LoadOptimum(OptFile, i);



                        //                        TimeLimit = size/2;

                        TestProblem.TimeLimit = TimeLimit * 1000;

                        if (File.Exists(OutFile))
                            continue;
                        Console.WriteLine(fileNameWithoutExtension);
                        PopupationSize = 800;
                        TestProblem.SolveFixSet(PopupationSize, K, MaxGeneratedSolutions, MaxBinVar, MaxStag, LimitPerFix);
                        //Console.WriteLine("Here++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                        TestProblem.SaveIntermediate(OutFile);
                            

                    }

                }
            }
               
            
            
        }
       

        public void LoadAllResults(string FileName, int NumInstances,out double[][] Results) {

            string[] Lines = File.ReadAllLines(FileName);
            string[] words;
            double cValue;

            Results = new double[NumInstances][];

            for (int i = 0; i < NumInstances; i++) {

                words = Lines[i+1].Split(',');

//                InstanceNames[i] = words[0];

                Results[i] = new double[ words.Length];
                double.TryParse(words[1], out Results[i][0]);
                for (int j = 0; j < words.Length; j++) {

                    double.TryParse(words[j], out cValue);
                    Results[i][j] = cValue;
                }

            }

        }

        public int GetBestSolution(string FileName) {

            double tResult = 0;
            int Result = -1;
            string[] Lines = File.ReadAllLines(FileName);
            string[] words = Lines[Lines.Length - 1].Split(' ');


            if (double.TryParse(words[0], out tResult))
            {
                Result = (int)Math.Round(tResult);
            }

            return Result;
        }

        public double GetTimeBestSolution(string FileName)
        {

            double Result = -1;
            string[] Lines = File.ReadAllLines(FileName);
            string[] words = Lines[Lines.Length - 1].Split(' ');


            if (!double.TryParse(words[2], out Result))
            {
                Result = double.MinValue;
            }

            return Result;
        }


        

        public void GenerateTables(string OutputFile)
        {

            MFSS_mKPCPProblem TestProblem;
            MFSS_mKPCPInstance TestInstance = new MFSS_mKPCPInstance();
            
            int PopupationSize = 200;
            int MaxItems = 1000;
            double LimitPerFix = 0.1;
            int MaxGeneratedSolutions = 500000;
            int MaxStag = 50;
            long TimeLimit = 600;
            int K = 5;

            StreamWriter F = new StreamWriter(OutputFile);
            string configString = "Res_P" + PopupationSize + "_MI" + MaxItems + "_TLF" + LimitPerFix + "_K" + K + "_M" + MaxStag;




            InitFiles();
            string cResDirectory;
            string MethodsFile;
            string tFileName;
            double[][] MethodValues;
            double[] cValues;
            int cSolution;
            double Sum = 0;
            int[] Best = new int[mNumInstances];
            int counter;
            double[] Avg = new double[mNumInstances];
            string[] InstanceNames;
            string Line;
            int NumBest = -1;
            int NumAvg = -1;
            double[] AvgAvgMethod = new double[mNumInstances];
            double[] AvgBestMethod = new double[mNumInstances];

            int[] CounterAvgBetter = new int[mNumInstances];
            int[] CounterAvgWorse = new int[mNumInstances];
            int[] CounterAvgEqual = new int[mNumInstances];


            int[] CounterBestBetter = new int[mNumInstances];
            int[] CounterBestWorse = new int[mNumInstances];
            int[] CounterBestEqual = new int[mNumInstances];
            double[] AvgAllMethods;
            double AVGBestAll;
            double AVGAvgAll;

            int StartTableTime = 8;
            int in_couter;
            string cLine;
            foreach (string itype in mProbleTypes)
            {

                MethodsFile = mInstancesDirectory + "Res_MFSS_mKPCP" + itype + ".csv";
                LoadAllResults(MethodsFile, 40, out MethodValues);

                in_couter = 0;
                F.WriteLine("*******" + itype + "*******");
                foreach (int isize in mProbleSizes)
                {
                    for (int iinstance = 0; iinstance < 10; iinstance++)
                    {
                        counter = 0;

                        Sum = 0;

                        for (int s = 0; s < 10; s++)
                        {
                            cResDirectory = mInstancesDirectory + configString + "\\\\";
                            tFileName = cResDirectory + "Res_" + itype + "_" + isize + "_"+ iinstance+ "_" + s + ".txt";

                            if (File.Exists(tFileName))
                            {

                                cSolution = GetBestSolution(tFileName);

                                Sum += cSolution;
                                if (cSolution > Best[in_couter])
                                    Best[in_couter] = cSolution;
                                counter++;
                            }

                        }

                        Avg[in_couter] = Sum / counter;
                        in_couter++;
                    }


                    


                }

                for (int i = 0; i < 40; i++)
                {
                    cLine = "";
                    for (int j = 0; j < MethodValues[i].Length; j++)
                    {
                        cLine += MethodValues[i][j] + " & ";
                    }
                    cLine += Best[i] + " & ";
                    cLine += Avg[i].ToString("0.0") + "\\\\ ";

                    F.WriteLine(cLine);
                }

            }
            F.Close();

            
        
        }
       

        long GetValueForTime(long iTime, List<long[]> TimeValues)
        {

            long Result = TimeValues[0][1];

            for (int i = 0; i < TimeValues.Count; i++)
                if (TimeValues[i][0] <= iTime)
                    Result = TimeValues[i][1];

            return Result;
        }


        public void SolvePopsizeAll()
        {

            MFSS_mKPCPProblem TestProblem;
            MFSS_mKPCPInstance TestInstance = new MFSS_mKPCPInstance();

            int[] PopupationSize = { 50, 100, 200, 500 };
            int MaxBinVar =250;
            double LimitPerFix = 0.1;
            int MaxGeneratedSolutions = 500000;
            int MaxStag = 50;
            long TimeLimit = 600;
            int K = 5;



            InitFiles();
            string cResDirectory;
            string configString = "Res_P" + PopupationSize + "_MI" + MaxBinVar + "_TLF" + LimitPerFix + "_K" + K + "_M" + MaxStag;
            string OutFile;
            string tFileName;
            //            int s = 1;
            string cDirectory;
            string[] filePaths;
            int size = 1000;


            for (int ip = 0; ip < PopupationSize.Length; ip++)
            {
                for (int s = 0; s < 10; s++)
                {
                    //                foreach (int size in mProbleSizes)
                    //                {
                    foreach (string ptype in mProbleTypes)
                    {

                        configString = "Res_P" + PopupationSize[ip] + "_MI" + MaxBinVar + "_TLF" + LimitPerFix + "_K" + K + "_M" + MaxStag;

                        cResDirectory = mInstancesDirectory + configString + "\\\\";
                        if (!System.IO.Directory.Exists(cResDirectory))
                        {
                            System.IO.Directory.CreateDirectory(cResDirectory);
                        }




                        cDirectory = mInstancesDirectory + ptype + "\\\\" + size + "\\\\";
                        filePaths = Directory.GetFiles(cDirectory);

                        for (int fi = 0; fi < 1; fi++)
                        {

                            TestInstance = new MFSS_mKPCPInstance();
                            TestInstance.Load(filePaths[fi]);
                            TestProblem = new MFSS_mKPCPProblem(TestInstance);
                            TestProblem.InitRandom(s);
                            //   OutFile = mInstancesDirectory+"Results\\\\" + "Res_" + ptype + "_" + size + "_"+fi+".txt" ;
                            OutFile = cResDirectory + "Res_" + ptype + "_" + size + "_" + fi + "_" + s + ".txt";
                            // TestInstance.LoadOptimum(OptFile, i);



                            //                        TimeLimit = size/2;

                            TestProblem.TimeLimit = TimeLimit * 1000;

                            if (File.Exists(OutFile))
                                continue;
                            TestProblem.SolveFixSet(PopupationSize[ip], K, MaxGeneratedSolutions, MaxBinVar, MaxStag, LimitPerFix);
                            TestProblem.SaveIntermediate(OutFile);

                        }

                    }
                }
            }
        }

        public void SolveMaxBinVarAll()
        {

            MFSS_mKPCPProblem TestProblem;
            MFSS_mKPCPInstance TestInstance = new MFSS_mKPCPInstance();

            int PopupationSize = 200;
            int[] MaxBinVar = { 250, 500, 1000 };
            double LimitPerFix = 0.1;
            int MaxGeneratedSolutions = 500000;
            int MaxStag = 50;
            long TimeLimit = 600;
            int K = 5;



            InitFiles();
            string cResDirectory;
            string configString = "Res_P" + PopupationSize + "_MI" + MaxBinVar + "_TLF" + LimitPerFix + "_K" + K + "_M" + MaxStag;
            string OutFile;
            string tFileName;
            //            int s = 1;
            string cDirectory;
            string[] filePaths;
            int size = 1000;


            for (int im = 0; im < MaxBinVar.Length; im++)
            {
                for (int s = 0; s < 10; s++)
                {
                    //                foreach (int size in mProbleSizes)
                    //                {
                    foreach (string ptype in mProbleTypes)
                    {

                        configString = "Res_P" + PopupationSize + "_MI" + MaxBinVar[im] + "_TLF" + LimitPerFix + "_K" + K + "_M" + MaxStag;

                        cResDirectory = mInstancesDirectory + configString + "\\\\";
                        if (!System.IO.Directory.Exists(cResDirectory))
                        {
                            System.IO.Directory.CreateDirectory(cResDirectory);
                        }




                        cDirectory = mInstancesDirectory + ptype + "\\\\" + size + "\\\\";
                        filePaths = Directory.GetFiles(cDirectory);

                        for (int fi = 0; fi < 1; fi++)
                        {

                            TestInstance = new MFSS_mKPCPInstance();
                            TestInstance.Load(filePaths[fi]);
                            TestProblem = new MFSS_mKPCPProblem(TestInstance);
                            TestProblem.InitRandom(s);
                            //   OutFile = mInstancesDirectory+"Results\\\\" + "Res_" + ptype + "_" + size + "_"+fi+".txt" ;
                            OutFile = cResDirectory + "Res_" + ptype + "_" + size + "_" + fi + "_" + s + ".txt";
                            // TestInstance.LoadOptimum(OptFile, i);



                            //                        TimeLimit = size/2;

                            TestProblem.TimeLimit = TimeLimit * 1000;

                            if (File.Exists(OutFile))
                                continue;
                            TestProblem.SolveFixSet(PopupationSize, K, MaxGeneratedSolutions, MaxBinVar[im], MaxStag, LimitPerFix);
                            TestProblem.SaveIntermediate(OutFile);

                        }

                    }
                }
            }
        }


    }
}
