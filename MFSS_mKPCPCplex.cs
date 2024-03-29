using ILOG.Concert;
using ILOG.CPLEX;

namespace MFSS_mKPCP
{
    class MFSS_mKPCPCplex
    {

        MFSS_mKPCPInstance mInstance;

        private IIntVar[] x; 
        Cplex cplex;
        bool mSolved;
        double mTimeLimit;
        MFSS_mKPCPSolution mSolution;


        public MFSS_mKPCPSolution Solution
        {
            get { return mSolution; }
        }

        public double TimeLimit
        {
            get { return mTimeLimit; }
            set { mTimeLimit = value; }
        }


        public MFSS_mKPCPCplex(MFSS_mKPCPInstance iInstance)
        {
            mInstance = iInstance;
            mTimeLimit = 0;
        }

        public void GenerateModel(MFSS_mKPCPCplexEXT Fix = null)
        {

            cplex = new Cplex();
            GenerateVariables();
            GenerateConstraints();
            GenerateObjective();
            if (Fix != null)
            {
                GenerateFixConstraints(Fix);
            }

        }

        public void GenerateFixConstraints(MFSS_mKPCPCplexEXT Fix)
        {

            ILinearIntExpr expr;

            if (Fix.mStates != null)
            {
                for (int i = 0; i < Fix.mStates.Length; i++)
                {

                    expr = cplex.LinearIntExpr();

                    switch (Fix.mStates[i])
                    {
                        case MFSS_mKPCPSolution.MFSS_mKPCPItemStatus.Unknown:
                            break;
                        case MFSS_mKPCPSolution.MFSS_mKPCPItemStatus.NotUsing:
                            expr.AddTerm(1, x[i]);
                            cplex.AddEq(0, expr);
                            break;
                        case MFSS_mKPCPSolution.MFSS_mKPCPItemStatus.Using:
                            expr.AddTerm(1, x[i]);
                            cplex.AddEq(1, expr);
                            break;
                    }
                }
            }

            if (Fix.mNumElements != -1)
            {

                expr = cplex.LinearIntExpr();
                for (int i = 0; i < mInstance.NumItems; i++)
                {

                    expr.AddTerm(x[i], 1);
                }

                cplex.AddEq(Fix.mNumElements, expr);

            }


        }

        private void GenerateVariables()
        {

            x = new IIntVar[mInstance.NumItems];


            int[] xlb = new int[mInstance.NumItems];
            int[] xub = new int[mInstance.NumItems];


            for (int i = 0; i < mInstance.NumItems; i++)
            {

                xlb[i] = 0;
                xub[i] = 1;
            }


            x = cplex.IntVarArray(mInstance.NumItems, xlb, xub);


            for (int i = 0; i < mInstance.NumItems; i++)
            {
                x[i].Name = "x" + (i);
            }


        }


        void GenerateObjective()
        {

            IObjective objective1;
            ILinearNumExpr expr = cplex.LinearNumExpr();

            expr = cplex.LinearNumExpr();

            for (int i = 0; i < mInstance.NumItems; i++)
            {
                expr.AddTerm(mInstance.ItemValues[i], x[i]);
            }

            objective1 = cplex.Minimize(expr);
            cplex.Add(objective1);
        }


        private void GenerateConstraints()
        {
            ILinearNumExpr expr = cplex.LinearNumExpr();

            expr = cplex.LinearNumExpr();
            for (int j = 0; j < mInstance.NumItems; j++)
            {
                expr.AddTerm(mInstance.ItemWeights[j], x[j]);
            }
            string capacityConstraintName = "Capacity_Constraint";
            cplex.AddGe(expr, mInstance.Capacity).Name = capacityConstraintName;

            for (int i = 0; i < mInstance.NumItems - mInstance.Delta - 1; i++)
            {
                for (int j = i + mInstance.Delta + 1; j < mInstance.NumItems; j++)
                {
                    ILinearIntExpr sequencingExpr = cplex.LinearIntExpr();

                    sequencingExpr.AddTerm(1, x[i]);
                    sequencingExpr.AddTerm(1, x[j]);

                    for (int k = i + 1; k < j; k++)
                    {
                        sequencingExpr.AddTerm(-1, x[k]);
                    }

                    string sequencingConstraintName = $"Sequencing_Constraint_{i}_{j}";
                    cplex.AddLe(sequencingExpr, 1).Name = sequencingConstraintName;
                }
            }
        }


        public void AddFixedConstraints(int[] fix)
        {
            for (int i = 0; i < fix.Length; i++)
            {
                if (fix[i] == 1 || fix[i] == 0)
                {
                    cplex.AddEq(x[i], fix[i]);
                }
            }
        }

        public void Solve(MFSS_mKPCPCplexEXT Fix = null)
        {

            GenerateModel(Fix);

            if (Fix != null)
            {

                if (Fix.mHotStart != null)
                {

                    IIntVar[] startvar = new IIntVar[mInstance.NumItems];
                    double[] startval = new double[mInstance.NumItems];

                    for (int i = 0; i < mInstance.NumItems; i++)
                    {

                        startvar[i] = x[i];
                        if (Fix.mHotStart.ItemStatuses[i] == MFSS_mKPCPSolution.MFSS_mKPCPItemStatus.Using)
                            startval[i] = 1;
                        else
                            startval[i] = 0;
                    }

                    cplex.AddMIPStart(startvar, startval, Cplex.MIPStartEffort.SolveMIP);
                }
            }

            cplex.ExportModel(@"C:\Users\AhmetCRBL\Desktop\MFSS_mKPCP\lpex1.lp");


            if (mTimeLimit > 0)
                cplex.SetParam(Cplex.Param.TimeLimit, mTimeLimit);
            cplex.SetOut(null);



            if (cplex.Solve())
            {
                mSolution = new MFSS_mKPCPSolution(mInstance);

                Console.WriteLine("Solution value: " + cplex.ObjValue + "  Thread :" + Thread.CurrentThread.ManagedThreadId + " Time Limit:" + mTimeLimit);

                double[] xres = cplex.GetValues(x);

                for (int i = 0; i < mInstance.NumItems; i++)
                {

                    if ((int)Math.Round(xres[i]) == 1)
                    {
                        //System.Console.WriteLine("x" + i + " " + xres[i]);
                        mSolution.SetStatusItem(i, MFSS_mKPCPSolution.MFSS_mKPCPItemStatus.Using);
                    }
                    else
                    {

                        mSolution.SetStatusItem(i, MFSS_mKPCPSolution.MFSS_mKPCPItemStatus.NotUsing);
                    }
                }
            }
            else
            {
                mSolved = false;
                mSolution = null;
                Console.WriteLine("Failed to find a solution.");
            }
            cplex.End();
        }



    }


}


