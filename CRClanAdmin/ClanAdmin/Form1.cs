using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CRLib;



namespace ClanAdmin
{
    public partial class Form1 : Form
    {
        private StatsWarLog statsWarLog;


        public Form1()
        {
            InitializeComponent();

            InitApp();
        }


        private void InitApp()
        {
            statsWarLog = new StatsWarLog();
            SetCaptionText();
        }


        private async void btnStart_Click(object sender, EventArgs e)
        {
            // get data from GUI           
            string filePath = textBoxKey.Text;
            string clanId = textBoxClanId.Text;          

            // set clan id and number of previous wars
            statsWarLog.SetClanId(clanId);
            int numWars = 5;
            if (Int32.TryParse(textBoxNumWars.Text, out numWars))
            {
                statsWarLog.SetNumWars(numWars);
            }

            // load key value
            statsWarLog.LoadKeyValue(filePath);
            if (statsWarLog.GetKeyVal() != null)
            {
                // request data to server
                string strError = await statsWarLog.RequestDataFromServer();
                if (strError == null)
                {
                    // create table and display data 
                    DataTable tableWarLog = statsWarLog.CreateTable();
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = tableWarLog;
                    dataGridView1.AutoResizeColumns();                    
                    // highlight players with good and bad performance
                    SetColorsOnTable(tableWarLog);

                    if (checkBoxHtml.Checked)
                    {
                        string HtmlBody = statsWarLog.ConvertTableToHtml(tableWarLog);
                        System.IO.File.WriteAllText(@"WarLogTable.html", HtmlBody);
                    }
                }
                else
                {
                    richTextBoxLog.AppendText(strError);
                    richTextBoxLog.AppendText(Environment.NewLine);
                }
            }
        }


        private void SetColorsOnTable(DataTable dt)
        {
            foreach (DataRow myRow in dt.Rows)
            {                
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
                        dataGridView1.Rows[dt.Rows.IndexOf(myRow)].Cells[dt.Columns.IndexOf(myColumn)].Style.BackColor = Color.Red;
                    else if (isCellGreen)                        
                        dataGridView1.Rows[dt.Rows.IndexOf(myRow)].Cells[dt.Columns.IndexOf(myColumn)].Style.BackColor = Color.LightGreen;
                    else if (isCellYellow)                        
                        dataGridView1.Rows[dt.Rows.IndexOf(myRow)].Cells[dt.Columns.IndexOf(myColumn)].Style.BackColor = Color.Yellow;                    
                }                
            }
        }


        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxKey.Text = openFileDialog.FileName;
            }
        }   
        

        private void SetCaptionText()
        {
            string captionText = @"Most recent war is displayed first.
Captions on table:
ce_#: cards earned during a war #
bt_#: battles played during a war # 
wi_#: wins during a war # 
cb_#: battles played during collection time of war # 
cell in red: participant didn't play his/her battle 
cell in green: participant won at least a battle
cell in yellow: lowest number of cards earned during collection time
-------------------------------------------------------------------------------------------------------------
";

            
            richTextBoxLog.Text = captionText;
        }
    }
}
