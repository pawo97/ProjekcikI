using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Shapes;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

namespace ProjektIPM
{
    public class DataChart
    {
        public string Shame;
        public string Markers;

        public void getChartData()
        {
            List<Customer> student = new List<Customer>();
            student.Add(new Customer("ala", 20));
            student.Add(new Customer("bela", 50));

            //(ColumnChart.Series[0] as ColumnSeries)
            //(Line.Series[0] as LinearAxis).ItemSource = student;
        }
    }
}
