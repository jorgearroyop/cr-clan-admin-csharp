using System;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;


namespace CRLib
{
    public class StatsCurrentWar : StatsWar
    {
        private CurrentWar cw;
        private string keyVal = null;        



        public override string LoadKeyValue(string path)
        {
            string error = null;

            try
            {                
                keyVal = File.ReadAllText(path); ;
            }
            catch (Exception e)
            {
                error = e.Message;
                Console.WriteLine(error);
            }

            return error;
        }



        public override async Task<string> RequestDataFromServer()
        {
            string error = null;

            string uri = "https://api.clashroyale.com/v1/clans/%232Y0L8VJ2/currentwar";
            HttpClient httpClient = new HttpClient();            
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", keyVal);
            string responseBody = await httpClient.GetStringAsync(uri);

            cw = JsonConvert.DeserializeObject<CurrentWar>(responseBody);

            // Output to the console
            //Console.WriteLine(responseBody);

            // Output a txt file
            //System.IO.File.WriteAllText(@"currentWar.txt", responseBody);

            // Output an XML file
            //XNode node = JsonConvert.DeserializeXNode(responseBody, "Root");
            //Console.WriteLine(node.ToString());
            //System.IO.File.WriteAllText(@"currentWar.xml", node.ToString());

            //return true;
            return error;
        }


        public override DataTable CreateTable()
        {
            //Create a DataTable
            DataTable table = new DataTable();
            // Define columns 
            table.Columns.Add("num", typeof(int));
            table.Columns.Add("name", typeof(string));
            table.Columns.Add("cardsEarned", typeof(int));
            table.Columns.Add("battlesPlayed", typeof(int));
            table.Columns.Add("wins", typeof(int));
            table.Columns.Add("collectionDayBattlesPlayed", typeof(int));
            // Add rows
            for (int i = 0; i < cw.clan.participants; i++)
            {
                table.Rows.Add(i + 1, cw.participants[i].name, cw.participants[i].cardsEarned, cw.participants[i].battlesPlayed, cw.participants[i].wins, cw.participants[i].collectionDayBattlesPlayed);
            }

            // Print table in console
            //foreach (DataRow dataRow in table.Rows)
            //{
            //    foreach (var item in dataRow.ItemArray)
            //    {
            //        Console.Write(item + "  |  ");
            //    }
            //    Console.WriteLine();
            //}

            return table;
        }

                       
        // tables styles: https://www.w3schools.com/html/html_tables.asp
        public override string ConvertTableToHtml(DataTable dt)
        {
            StringBuilder strHTMLBuilder = new StringBuilder();
            strHTMLBuilder.Append("<html >");
            strHTMLBuilder.Append("<head>");

            strHTMLBuilder.Append("<style>");
            strHTMLBuilder.Append("table {font-family: arial, sans-serif; border-collapse: collapse; table-layout: auto; width: 100%;}");
            strHTMLBuilder.Append("td, th {border: 1px solid #dddddd;text-align: left; padding: 3px; white-space: nowrap;}");
            strHTMLBuilder.Append("tr:nth-child(even) {background-color: #dddddd;}");
            strHTMLBuilder.Append("</style>");

            strHTMLBuilder.Append("</head>");
            strHTMLBuilder.Append("<body>");
            strHTMLBuilder.Append("<h2> Clan: Tines </h2>");
            strHTMLBuilder.Append("<p>Current War</p>");

            strHTMLBuilder.Append("<table>");

            strHTMLBuilder.Append("<tr >");
            foreach (DataColumn myColumn in dt.Columns)
            {
                strHTMLBuilder.Append("<th >");
                strHTMLBuilder.Append(myColumn.ColumnName);
                strHTMLBuilder.Append("</th>");
            }
            strHTMLBuilder.Append("</tr>");

            // TODO: Here maybe compare with a dictionary of column and rows of the last 3 participants
            foreach (DataRow myRow in dt.Rows)
            {
                strHTMLBuilder.Append("<tr >");
                foreach (DataColumn myColumn in dt.Columns)
                {
                    strHTMLBuilder.Append("<td >");
                    strHTMLBuilder.Append(myRow[myColumn.ColumnName].ToString());
                    strHTMLBuilder.Append("</td>");
                }
                strHTMLBuilder.Append("</tr>");
            }

            //Close tags
            strHTMLBuilder.Append("</table>");
            strHTMLBuilder.Append("</body>");
            strHTMLBuilder.Append("</html>");

            string Htmltext = strHTMLBuilder.ToString();
            return Htmltext;
        }


        public string GetKeyVal()
        {
            return keyVal;
        }
    }
}
