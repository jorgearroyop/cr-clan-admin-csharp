using CRLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRLib
{
    public abstract class StatsWar
    {
        public abstract void LoadKeyValue();

        public abstract Task<bool> RequestDataFromServer();

        public abstract DataTable CreateTable();

        public abstract string ConvertTableToHtml(DataTable dt);

        public abstract void OutputInHtmlFile();
    }
}
