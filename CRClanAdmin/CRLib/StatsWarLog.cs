using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Data;
using System.Globalization;
using System.IO;




namespace CRLib
{
    public class StatsWarLog : StatsWar
    {
        private WarLog wl;
        private int numWars = 5;
        private string keyVal = null;
        private string clanId = null;       



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
            string uri = GetWarLogUri();
            HttpClient httpClient = new HttpClient();

            try
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", keyVal);
                string responseBody = await httpClient.GetStringAsync(uri);

                wl = JsonConvert.DeserializeObject<WarLog>(responseBody);                
            }
            catch(Exception e)
            {
                error = e.Message + "\nMake sure your clan id is correct and that you ip has been added to the key created on CR Web API: https://developer.clashroyale.com";
                Console.WriteLine(e.Message);                
            }            
            return error;
        }


        public override DataTable CreateTable()
        {
            //Create a DataTable  
            DataTable table = new DataTable();
            List<string> pnames = new List<string>();
            int counter = 0;
            int colsPerWar = 4;

            // Define columns
            table.Columns.Add("number", typeof(int));
            table.Columns.Add("name", typeof(string));
            for (int k = 0; k < wl.items.Length; k++)
            {                
                table.Columns.Add("ce_" + (k + 1), typeof(int));            // cards earned
                table.Columns.Add("bt_" + (k + 1), typeof(int));            // battles   
                table.Columns.Add("wi_" + (k + 1), typeof(int));            // wins
                table.Columns.Add("cb_" + (k + 1), typeof(int));            // collection battles
            }

            //Add rows            
            for (int j = 0; j < wl.items.Length; j++)
            {
                for (int i = 0; i < wl.items[j].participants.Length; i++)
                {
                    string tempName = wl.items[j].participants[i].name;
                    int index = pnames.FindIndex(s => s.Equals(tempName));
                    DateTime tempDateTime = FormatDateCR(wl.items[j].createdDate);

                    // if name was not found
                    if (index < 0)
                    {
                        counter++;
                        pnames.Add(tempName);

                        DataRow dr = table.NewRow();
                        dr[0] = counter;
                        dr[1] = tempName;                        
                        dr[colsPerWar * j + 2] = wl.items[j].participants[i].cardsEarned;
                        dr[colsPerWar * j + 3] = wl.items[j].participants[i].battlesPlayed;
                        dr[colsPerWar * j + 4] = wl.items[j].participants[i].wins;
                        dr[colsPerWar * j + 5] = wl.items[j].participants[i].collectionDayBattlesPlayed;

                        table.Rows.Add(dr);
                    }
                    else
                    {                        
                        table.Rows[index][colsPerWar * j + 2] = wl.items[j].participants[i].cardsEarned;
                        table.Rows[index][colsPerWar * j + 3] = wl.items[j].participants[i].battlesPlayed;
                        table.Rows[index][colsPerWar * j + 4] = wl.items[j].participants[i].wins;
                        table.Rows[index][colsPerWar * j + 5] = wl.items[j].participants[i].collectionDayBattlesPlayed;
                    }
                }
            }

            CalculateTotals(table);

            return table;
        }



        // tables styles: https://www.w3schools.com/html/html_tables.asp
        public override string ConvertTableToHtml(DataTable dt)
        {
            StringBuilder strHTMLBuilder = new StringBuilder();
            strHTMLBuilder.Append("<html>");
            strHTMLBuilder.Append("<head>");

            strHTMLBuilder.Append("<style>");
            strHTMLBuilder.Append("table {font-family: arial, sans-serif; border: 1px solid black; table-layout: auto; width: 100%;}");
            strHTMLBuilder.Append("td, th {border: 1px solid #dddddd;text-align: left; padding: 3px; white-space: nowrap;}");
            strHTMLBuilder.Append("tr:nth-child(even) {background-color: #dddddd;}");
            strHTMLBuilder.Append("</style>");

            strHTMLBuilder.Append("</head>");
            strHTMLBuilder.Append("<body>");
            strHTMLBuilder.Append("<h2> Clan: Tines </h2>");
            strHTMLBuilder.Append("<h3>War Log: Previous " + wl.items.Length + " wars</h3>");

            strHTMLBuilder.Append("<table>");

            strHTMLBuilder.Append("<tr>");
            strHTMLBuilder.Append("<th colspan = \"2\" >  </th>");
            for (int i = 0; i < wl.items.Length; i++)
            {
                DateTime tempDateTime = FormatDateCR(wl.items[i].createdDate);
                string tempStr = tempDateTime.ToShortDateString();
                strHTMLBuilder.Append("<th colspan = \"4\" > War " + (i + 1) + ": " + tempStr + "</th>");
            }
            strHTMLBuilder.Append("</tr>");

            strHTMLBuilder.Append("<tr>");
            foreach (DataColumn myColumn in dt.Columns)
            {
                strHTMLBuilder.Append("<td>");
                strHTMLBuilder.Append(myColumn.ColumnName);
                strHTMLBuilder.Append("</td>");
            }
            strHTMLBuilder.Append("</tr>");


            foreach (DataRow myRow in dt.Rows)
            {
                strHTMLBuilder.Append("<tr>");
                foreach (DataColumn myColumn in dt.Columns)
                {
                    //Rules to highlight cells in html table

                    // check for participants that didn't play their battle
                    bool isCellRed = false;
                    bool isCellGreen = false;
                    bool isCellYellow = false;
                    string typeCol = myColumn.ColumnName.Substring(0, 2);
                    string cellVal = myRow[myColumn.ColumnName].ToString();

                    if (typeCol == "bt")
                    {
                        int val;
                        if (Int32.TryParse(cellVal, out val))
                        {
                            if (val == 0)
                                isCellRed = true;
                        }
                    }

                    if (typeCol == "wi")
                    {
                        int val;
                        if (Int32.TryParse(cellVal, out val))
                        {
                            if (val > 0)
                                isCellGreen = true;
                        }
                    }

                    if (typeCol == "ce")      // cards earned
                    {
                        string colName = myColumn.ColumnName;
                        string expression = "min([" + colName + "])";
                        int minVal = Convert.ToInt32(dt.Compute(expression, string.Empty));

                        int val;
                        if (Int32.TryParse(cellVal, out val))
                        {
                            if (val == minVal)
                                isCellYellow = true;
                        }
                    }

                    if (isCellRed)
                        strHTMLBuilder.Append("<td bgcolor = \"#FF0000\">");
                    else if (isCellGreen)
                        strHTMLBuilder.Append("<td bgcolor = \"#00FF00\">");
                    else if (isCellYellow)
                        strHTMLBuilder.Append("<td bgcolor = \"#FFFF00\">");
                    else
                        strHTMLBuilder.Append("<td >");

                    strHTMLBuilder.Append(myRow[myColumn.ColumnName].ToString());
                    strHTMLBuilder.Append("</td>");
                }
                strHTMLBuilder.Append("</tr>");
            }

            //Close tags
            strHTMLBuilder.Append("</table>");

            strHTMLBuilder.Append("<p>");
            strHTMLBuilder.Append("ce_#: cards earned during a war # <br>");
            strHTMLBuilder.Append("bt_#: battles played during a war # <br>");
            strHTMLBuilder.Append("wi_#: wins during a war # <br>");
            strHTMLBuilder.Append("cb_#: battles played during collection time of war # <br>");
            strHTMLBuilder.Append("cell in red: participant didn't play his/her battle <br>");
            strHTMLBuilder.Append("cell in green: participant won at least a battle <br>");
            strHTMLBuilder.Append("cell in yellow: lowest number of cards earned during collection time<br>");

            strHTMLBuilder.Append("</p>");

            strHTMLBuilder.Append("</body>");
            strHTMLBuilder.Append("</html>");

            string Htmltext = strHTMLBuilder.ToString();
            return Htmltext;
        }


        public string GetKeyVal()
        {
            return keyVal;
        }
        

        public void SetNumWars(int num)
        {
            numWars = num;
        }
        

        public void SetClanId(string cId)
        {
            if (cId.Contains("#"))
                clanId = cId.Substring(1);
            else
                clanId = cId;
        }
        

        private DateTime FormatDateCR(string strDate)
        {
            // incoming string: 20181002T021629.000Z
            // desired string: 2018-10-02T02:16:29.000Z
            string year = strDate.Substring(0, 4);
            string month = strDate.Substring(4, 2);
            string day = strDate.Substring(6, 2);
            string hour = strDate.Substring(9, 2);
            string min = strDate.Substring(11, 2);
            string sec = strDate.Substring(13, 2);
            string fffz = strDate.Substring(16, 4);

            string fullDate = year + "-" + month + "-" + day + "T" + hour + ":" + min + ":" + sec + "." + fffz;
            
            DateTime newDate = DateTime.ParseExact(fullDate, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);

            return newDate;
        }


        private string GetWarLogUri()
        {
            string uri = "https://api.clashroyale.com/v1/clans/%23" + clanId + "/warlog";
            
            if (numWars > 0)
            {                
                uri = uri + "?limit=" + numWars;
            }           

            return uri;
        }


        private void CalculateTotals(DataTable dt)
        {
            List<int> TotWins = new List<int>();
            int sum;

            foreach (DataRow myRow in dt.Rows)
            {
                sum = 0;
                foreach (DataColumn myColumn in dt.Columns)
                {
                    string typeCol = myColumn.ColumnName.Substring(0, 2);                    

                    if (typeCol == "wi")
                    {
                        string cellVal = myRow[myColumn.ColumnName].ToString();
                        int val;
                        if (Int32.TryParse(cellVal, out val))
                        {
                            sum += val;
                        }
                    }
                }
                TotWins.Add(sum);
            }            

            dt.Columns.Add("SumWins", typeof(int));
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i][dt.Columns.Count - 1] = TotWins[i];
            }
        }
    }
}
