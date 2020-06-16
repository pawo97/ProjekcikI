using Microsoft.Toolkit.Uwp.UI.Controls;
using Newtonsoft.Json;
using SharpDX.WIC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace ProjektIPM
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class SecondPage : Page
    {
        private string currencyMain;
        public ViewModelSecondPage viewModel2 = new ViewModelSecondPage();
        //List<Rate> allRates = new List<Rate>();

        public SecondPage()
        {
            this.InitializeComponent();
            Load("listOfCurrencies2.JSON");
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is string && !string.IsNullOrWhiteSpace((string)e.Parameter))
            {
                currencyMain = e.Parameter.ToString();
                currentCurrency.Text = $"Historia waluty: {e.Parameter.ToString()} /PLN";
            }
            else
            {
                currentCurrency.Text = "Historia waluty: /PLN";
            }
            base.OnNavigatedTo(e);
        }
        //Dodanie i uzupełnienie tabeli
        public static void FillDataGrid(DataTable table, DataGrid grid)
        {
            grid.Columns.Clear();
            grid.AutoGenerateColumns = false;
            for (int i = 0; i < table.Columns.Count; i++)
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = table.Columns[i].ColumnName,
                    Binding = new Binding { Path = new PropertyPath("[" + i.ToString() + "]") }
                });
            }

            var collection = new ObservableCollection<object>();
            foreach (DataRow row in table.Rows)
            {
                collection.Add(row.ItemArray);
            }

            grid.ItemsSource = collection;
        }
        private DataTable GetDataTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Dzien", typeof(string));
            dt.Columns.Add("Kurs", typeof(string));
            //items = new List<CurrencyView>();
            for (int i = 0; i < this.viewModel2.ItemsChart.Count; i++)
            {
                Rate r = this.viewModel2.ItemsChart[i];
                dt.Rows.Add(r.effectiveDate.ToString("yyyy-MM-dd"), r.mid);
            }
            return dt;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            getValues();
        }
        public async void getValues()
        {
            this.viewModel2.ItemsChart = new List<Rate>();
            System.Diagnostics.Debug.WriteLine(this.viewModel2.FirstDate + " | " + this.viewModel2.SecondDate);
            try
            {
                string[] pom = this.viewModel2.FirstDate.Split("-");
                DateTime a = new DateTime(Convert.ToInt32(pom[0]), Convert.ToInt32(pom[1]), Convert.ToInt32(pom[2]));

                pom = this.viewModel2.SecondDate.Split("-");
                DateTime b = new DateTime(Convert.ToInt32(pom[0]), Convert.ToInt32(pom[1]), Convert.ToInt32(pom[2]));

                TimeSpan s = b.Subtract(a);
                int time = s.Days;
                int interval = s.Days / 200;
                int counter = -200;
                DateTime actual = a;
                DateTime date = b;
                DateTime lastDate = a;
                string urlEuro = "";
                List<int> values = new List<int>();
                string responseBody = "";
                HttpClient client = new HttpClient();
                bool status = true;

                if (time < 0) status = false;
                if (a < new DateTime(2002, 2, 1)) status = false;

                if(status)
                {
                    this.viewModel2.Start = 0;
                    this.viewModel2.Progress = 0;
                    this.viewModel2.Status = "";
                    //this.viewModel2.Finish = 10;
                    if (Math.Abs(time) < 200)
                    {
                        //System.Diagnostics.Debug.WriteLine("Przedzial mniejszy niz 200 dni");
                        urlEuro = "https://api.nbp.pl/api/exchangerates/rates/a/" + currencyMain + "/" + actual.ToString("yyyy-MM-dd") + "/" + date.ToString("yyyy-MM-dd") + "/";
                        HttpResponseMessage response = await client.GetAsync(urlEuro);
                        response.EnsureSuccessStatusCode();
                        responseBody = await response.Content.ReadAsStringAsync();

                        Currency c = new Currency();
                        c = JsonConvert.DeserializeObject<Currency>(responseBody);
                        foreach (Rate r in c.rates)
                        {
                            //System.Diagnostics.Debug.WriteLine(r.mid + " -> ");
                            this.viewModel2.ItemsChart.Add(new Rate(r.mid, r.effectiveDate));
                        }
                        this.viewModel2.Status = "Pobrano";
                        this.viewModel2.Progress = 20;
                    }
                    else
                    {
                        this.viewModel2.Start = 0;
                        this.viewModel2.Progress = 0;
                        this.viewModel2.Status = "Pobieranie...";
                        int counterTime = this.viewModel2.Finish / interval;
                        System.Diagnostics.Debug.WriteLine(counterTime + " <> " + interval);
                        actual = b.AddDays(counter);
                        date = b;
                        do
                        {
                            
                            System.Diagnostics.Debug.WriteLine(counter + " | " + actual + " | " + date);
                            if (actual < lastDate)
                            {
                                time = b.Subtract(a).Days;
                                actual = b.AddDays(-time);
                                System.Diagnostics.Debug.WriteLine(counter + " | " + actual + " | " + date);
                            }
                            urlEuro = "https://api.nbp.pl/api/exchangerates/rates/a/" + currencyMain + "/" + actual.ToString("yyyy-MM-dd") + "/" + date.ToString("yyyy-MM-dd") + "/";
                            HttpResponseMessage response = await client.GetAsync(urlEuro);
                            response.EnsureSuccessStatusCode();
                            responseBody = await response.Content.ReadAsStringAsync();
                            this.viewModel2.Progress += counterTime;
                            Currency c = new Currency();
                            c = JsonConvert.DeserializeObject<Currency>(responseBody);
                            foreach (Rate r in c.rates)
                            {
                                //System.Diagnostics.Debug.WriteLine(r.mid + " -> ");
                                this.viewModel2.ItemsChart.Add(new Rate(r.mid, r.effectiveDate));
                            }

                            System.Threading.Thread.Sleep(500);
                            System.Diagnostics.Debug.WriteLine(counter + " | " + actual + " | " + date);
                            date = date.AddDays(counter);
                            actual = actual.AddDays(counter);
                            System.Diagnostics.Debug.WriteLine(counter + " | " + actual + " | " + date);
                            if (actual < lastDate)
                            {
                                time = date.Subtract(lastDate).Days;
                                actual = date.AddDays(-time);

                                urlEuro = "https://api.nbp.pl/api/exchangerates/rates/a/" + currencyMain + "/" + actual.ToString("yyyy-MM-dd") + "/" + date.ToString("yyyy-MM-dd") + "/";
                                response = await client.GetAsync(urlEuro);
                                response.EnsureSuccessStatusCode();
                                responseBody = await response.Content.ReadAsStringAsync();

                                c = new Currency();
                                c = JsonConvert.DeserializeObject<Currency>(responseBody);
                                foreach (Rate r in c.rates)
                                {

                                    this.viewModel2.ItemsChart.Add(new Rate(r.mid, r.effectiveDate));
                                }
                                break;
                            }
                        } while (date > lastDate);
                        this.viewModel2.Status = "Pobrano";
                        this.viewModel2.Progress = this.viewModel2.Finish;
                    }
                }

                //Posortować listę po dacie
                this.viewModel2.ItemsChart = this.viewModel2.ItemsChart.OrderBy(x => x.effectiveDate.Date).ToList();

                //Stworzenie tabeli
                DataTable dt = GetDataTable();
                FillDataGrid(dt, dataGridCurrency);
                dataGridCurrency.ItemsSource = dt.DefaultView;

                //Tworzenie wykresu
                //System.Threading.Thread.Sleep(2500);
                getChartData(this.viewModel2.ItemsChart);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        public void getChartData(List<Rate> rate)
        {
            List<Customer> lista = new List<Customer>();
            foreach(Rate r in rate)
            {
                lista.Add(new Customer(r.effectiveDate.ToString("yyyy-MM-dd"), r.mid));
            }
            (LineChart.Series[0] as LineSeries).ItemsSource = lista;
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(LineChart);


            
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        public async void Load(string fileName)
        {
            var list = new List<Rate>();
            // Check if we had previously Save information of our friends
            // previously
            try
            {
                Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile sampleFile = await storageFolder.GetFileAsync("sampleChart.json");
                var text = await FileIO.ReadTextAsync(sampleFile);
                list = JsonConvert.DeserializeObject<List<Rate>>(text);
                this.viewModel2.ItemsChart = list;
                System.Diagnostics.Debug.WriteLine("Wielkosc listy DRUGA STRONA: " + list.Count);
                DataTable dt = GetDataTable();
                FillDataGrid(dt, this.dataGridCurrency);
                dataGridCurrency.ItemsSource = dt.DefaultView;
                getChartData(this.viewModel2.ItemsChart);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
                     
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            CoreApplication.Exit();
        }
    }
}
