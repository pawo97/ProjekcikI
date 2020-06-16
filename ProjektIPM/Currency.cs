using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektIPM
{
    public class Currency
    {
        public string table;
        public string currency;
        public string code;
        public List<Rate> rates;
        public Currency() { }
        public Currency(string table, string currency, string code, DateTime day)
        {
            this.table = table;
            this.currency = currency;
            this.code = code;
            this.rates = new List<Rate>();
            this.rates.Add(new Rate(0, day));
        }
        public override string ToString()
        {
            return this.table + " " + this.currency + this.code;
        }
    }
}