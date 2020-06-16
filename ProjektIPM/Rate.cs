using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektIPM
{
    public class Rate
    {
        public string no;
        public DateTime effectiveDate;
        public double mid;
        public Rate() { }
        public Rate(double mid, DateTime effectiveDate)
        {
            this.mid = mid;
            this.effectiveDate = effectiveDate;
        }
    }
}
