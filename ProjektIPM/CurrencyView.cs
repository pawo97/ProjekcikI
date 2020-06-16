using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektIPM
{
    public class CurrencyView : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string CrCode { get; set; }
        public double Mid { get; set; }

        public int MidExchanger { get; set; }

        public CurrencyView() { }
        public CurrencyView(string CrCode, string Name, double Mid, int MidExchanger)
        {
            this.Name = Name;
            this.CrCode = CrCode;
            this.Mid = Mid;
            this.MidExchanger = MidExchanger;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return this.Name + "\n" + this.Mid + " Przelicznik: " + this.MidExchanger;
        }
    }
}

