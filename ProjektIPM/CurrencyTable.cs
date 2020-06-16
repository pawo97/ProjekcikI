using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektIPM
{
    public class CurrencyTable
    {
        public string table;
        public string no;
        public string effectiveDate;
        public List<TableRate> rates;
        public CurrencyTable() { }
    }
}
