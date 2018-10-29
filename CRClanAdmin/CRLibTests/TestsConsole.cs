/*
*/


using System;
using System.Data;
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



        static async void TestWarLogOutputInHtml()
        {
            // set clan id and number of previous wars
            statsWarLog.SetNumWars(5);
            statsWarLog.SetClanId("2Y0L8VJ2");

            // load key value
            string filePath = @"key.txt";
            statsWarLog.LoadKeyValue(filePath);
            if (statsWarLog.GetKeyVal() != null)
            {
                // request data to server
                string strError = await statsWarLog.RequestDataFromServer();
                if (strError == null)
                {
                    // create table and convert it to html
                    DataTable tableWarLog = statsWarLog.CreateTable();
                    string HtmlBody = statsWarLog.ConvertTableToHtml(tableWarLog);
                    System.IO.File.WriteAllText(@"WarLogTable.html", HtmlBody);
                }
            }
        }


        static async void TestStatsCurrentWar()
        {
            // load key value
            string filePath = @"key.txt";
            statsCurrentWar.LoadKeyValue(filePath);
            if (statsCurrentWar.GetKeyVal() != null)
            {
                // request data to server
                string strError = await statsCurrentWar.RequestDataFromServer();
                if (strError == null)
                {
                    // create table and convert it to html
                    DataTable tableCurrentWar = statsCurrentWar.CreateTable();
                    string HtmlBody = statsCurrentWar.ConvertTableToHtml(tableCurrentWar);
                    System.IO.File.WriteAllText(@"currentWarDataTable.html", HtmlBody);
                }
            }
        }
    }
}
