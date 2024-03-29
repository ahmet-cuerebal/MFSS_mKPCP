using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFSS_mKPCP
{
    class Program
    {
        static void Main(string[] args)
        {


            MFSS_mKPCPInstance TestInstance = new MFSS_mKPCPInstance();


            MFSS_mKPCPExperiments Exp = new MFSS_mKPCPExperiments();
            //                                    Exp.GenerateTables("Table.txt");
            //Exp.SolveMaxBinVarAll();
            //Exp.SolvePopsizeAll();
            Exp.SolveAll();

            //                        Exp.SolveParamTest();
            //   Exp.GenerateTimeTable("TableTime.txt");
            //            Exp.GenerateParamResultFile("ResFile.txt",3,1);

            /*
            Exp.LoadAllRuns("Resmknapcb9_0_25_", "c:\\primeri\\MDKP\\Res_P100_MI25_TLF0.1_K5_M25\\", ".\\Res\\", "P100_MI25_TLF0.1_K5_M25", 10);
            Exp.LoadAllRuns("Resmknapcb9_0_25_", "c:\\primeri\\MDKP\\Res_P50_MI25_TLF0.1_K5_M25\\", ".\\Res\\", "P50_MI25_TLF0.1_K5_M25", 10);
            Exp.LoadAllRuns("Resmknapcb9_0_25_", "c:\\primeri\\MDKP\\Res_P200_MI25_TLF0.1_K5_M25\\", ".\\Res\\", "P200_MI25_TLF0.1_K5_M25", 10);
            */
            /*
            Exp.LoadAllRuns("Resmknapcb9_0_50_", "c:\\primeri\\MDKP\\", "P100_MI25_TLF0.1_K5_M50", ".\\Res\\", 10);
            Exp.LoadAllRuns("Resmknapcb9_0_50_", "c:\\primeri\\MDKP\\", "P100_MI50_TLF0.1_K5_M50", ".\\Res\\", 10);
            Exp.LoadAllRuns("Resmknapcb9_0_50_", "c:\\primeri\\MDKP\\", "P100_MI100_TLF0.1_K5_M50", ".\\Res\\", 10);


            Exp.LoadAllRuns("Resmknapcb9_0_50_", "c:\\primeri\\MDKP\\", "P50_MI100_TLF0.1_K5_M50", ".\\Res\\", 10);
            Exp.LoadAllRuns("Resmknapcb9_0_50_", "c:\\primeri\\MDKP\\", "P100_MI100_TLF0.1_K5_M50", ".\\Res\\", 10);
            Exp.LoadAllRuns("Resmknapcb9_0_50_", "c:\\primeri\\MDKP\\", "P200_MI100_TLF0.1_K5_M50", ".\\Res\\", 10);

            */
            /*
            Exp.LoadAllRuns("Resmknapcb9_0_50_", "c:\\primeri\\MDKP\\", "P100_MI25_TLF0.1_K5_M50", ".\\Res\\", 10, 116056);
            Exp.LoadAllRuns("Resmknapcb9_0_50_", "c:\\primeri\\MDKP\\", "P100_MI50_TLF0.1_K5_M50", ".\\Res\\", 10, 116056);
            Exp.LoadAllRuns("Resmknapcb9_0_50_", "c:\\primeri\\MDKP\\", "P100_MI100_TLF0.1_K5_M50", ".\\Res\\", 10, 116056);



            Exp.LoadAllRuns("Resmknapcb5_0_50_", "c:\\primeri\\MDKP\\", "P25_MI25_TLF0.1_K5_M50", ".\\Res\\", 10, 59187);
            Exp.LoadAllRuns("Resmknapcb5_0_50_", "c:\\primeri\\MDKP\\", "P50_MI25_TLF0.1_K5_M50", ".\\Res\\", 10, 59187);
            Exp.LoadAllRuns("Resmknapcb5_0_50_", "c:\\primeri\\MDKP\\", "P100_MI25_TLF0.1_K5_M50", ".\\Res\\", 10, 59187);
            Exp.LoadAllRuns("Resmknapcb5_0_50_", "c:\\primeri\\MDKP\\", "P200_MI25_TLF0.1_K5_M50", ".\\Res\\", 10, 59187);
            */

            System.Console.ReadKey();
        }
    }
}
