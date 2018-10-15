/*
*/


using System;
using CRLib;


namespace CRLibTests
{
    class TestsConsole
    {
        private static StatsWarLog statsWarLog;
        private static StatsCurrentWar statsCurrentWar;


        static void Main()
        {
            statsWarLog = new StatsWarLog();
            statsCurrentWar = new StatsCurrentWar();

            // Tests
            TestWarLogOutputInHtml();

            TestStatsCurrentWar();

            // Exit console application
            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }



        static void TestWarLogOutputInHtml()
        {
            statsWarLog.OutputInHtmlFile();
        }


        static void TestStatsCurrentWar()
        {
            statsCurrentWar.OutputInHtmlFile();
        }
    }
}
