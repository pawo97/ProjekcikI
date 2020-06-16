using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Net.Http;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using System.Data;
using System.Collections.ObjectModel;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.ApplicationModel.Core;
using Windows.Storage;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace ProjektIPM
{

    public sealed partial class MainPage : Page
    {
        //List<CurrencyView> items = new List<CurrencyView>();
        List<String> itemsData = new List<String>();
        List<Currency> tableA;
        List<CurrencyTable> tableB;
        ViewModel ViewModel = new ViewModel();


        public MainPage()
        {
            this.InitializeComponent();
            try
            {
                //Wczytanie danych
                InicializeDataList();
                CreateCurrencies();
                Load("listOfCurrencies.JSON");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
        }

        private void InicializeDataList()
        {
            //this.ViewModel.DateOfPublication = DateTime.Today.ToString("yyyy-MM-dd");
            string todayD = DateTime.Today.ToString("yyyy-MM-dd");
            TimeSpan span = DateTime.Today.Subtract(new DateTime(2002, 2, 01));
            int time = span.Days;
            for (int d = time; d >= 0; d--)
            {
                string ago = (DateTime.Today.AddDays(-d)).ToString("yyyy-MM-dd");
                itemsData.Add(ago);
            }
            Datas.ItemsSource = itemsData;
        }

        private void Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataRowView currency = (DataRowView)dataGrid.SelectedItem;
                Object[] c = currency.Row.ItemArray;
                System.Diagnostics.Debug.WriteLine("Heheszki " + c[2].ToString());
                this.Frame.Navigate(typeof(SecondPage), c[2].ToString());
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Blad ");
            }
            
        }

        private async void Datas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ViewModel.DateOfPublication = (string)Datas.SelectedItem;
            await GetCurrenciesFromDate();
            if (this.ViewModel.Items.Count == 0) this.ViewModel.Exist = "Brak Danych";
            else this.ViewModel.Exist = "";
        }
        private async Task CreateCurrencies()
        {
            //items = new List<CurrencyView>();
            tableA = new List<Currency>();
            HttpClient client = new HttpClient();
            string url = "https://api.nbp.pl/api/exchangerates/tables/A/";

            try
            {
                HttpResponseMessage response2 = await client.GetAsync(url);
                response2.EnsureSuccessStatusCode();
                string responseBody2 = await response2.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine(responseBody2);
                tableB = new List<CurrencyTable>();
                tableB = JsonConvert.DeserializeObject<List<CurrencyTable>>(responseBody2);


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Nie pobrano");
            }
        }

        private async Task GetCurrenciesFromDate()
        {
            System.Diagnostics.Debug.WriteLine("Pobrano");
            String text = Datas.SelectedItems[0].ToString();
            HttpClient client = new HttpClient();
            bool exist = false;
            tableA = new List<Currency>();
            this.ViewModel.Items = new List<CurrencyView>();

            string responseBody = "";
            string[] pom = text.Split('-');
            DateTime a = new DateTime(Convert.ToInt32(pom[0]), Convert.ToInt32(pom[1]), Convert.ToInt32(pom[2]));
            int i = 0;

            foreach (CurrencyTable cr in tableB)
            {
                foreach (TableRate r in cr.rates)
                {
                    //System.Diagnostics.Debug.WriteLine(cr.rates.currency + " | ");
                    try
                    {
                        string urlEuro = "https://api.nbp.pl/api/exchangerates/rates/a/" + r.code.ToLower() + "/" + a.ToString("yyyy-MM-dd") + "/";
                        //System.Diagnostics.Debug.WriteLine(urlEuro);
                        HttpResponseMessage response = await client.GetAsync(urlEuro);
                        response.EnsureSuccessStatusCode();
                        responseBody = await response.Content.ReadAsStringAsync();
                        tableA.Add(new Currency());
                        tableA[i] = JsonConvert.DeserializeObject<Currency>(responseBody);
                        i++;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Brak Danych");
                    }
                    
                }
            }

            foreach (Currency cr in tableA)
            {
                foreach (Rate c in cr.rates)
                {
                    System.Diagnostics.Debug.WriteLine(cr.currency);
                    if (c.effectiveDate.ToString("yyyy-MM-dd").Equals(text))
                    {
                        if (c.mid > 1)
                        {
                            this.ViewModel.Items.Add(new CurrencyView(cr.code, cr.currency, c.mid, 1));
                        }
                        else if (c.mid < 1 && c.mid > 0.1)
                        {
                            this.ViewModel.Items.Add(new CurrencyView(cr.code, cr.currency, c.mid * 10, 10));
                        }
                        else if (c.mid < 0.1 && c.mid > 0.01)
                        {
                            this.ViewModel.Items.Add(new CurrencyView(cr.code, cr.currency, c.mid * 100, 100));
                        }
                        else if (c.mid < 0.01 && c.mid > 0.001)
                        {
                            this.ViewModel.Items.Add(new CurrencyView(cr.code, cr.currency, c.mid * 1000, 1000));
                        }
                        else
                        {
                            this.ViewModel.Items.Add(new CurrencyView(cr.code, cr.currency, c.mid * 10000, 10000));
                        }
                    }
                }
            }
            //System.Diagnostics.Debug.WriteLine("Przeszlo petle");
            //Stworzenie tabeli
            DataTable dt = GetDataTable();
            FillDataGrid(dt, dataGrid);
            dataGrid.ItemsSource = dt.DefaultView;

            //Sprawdzenie czy dane istnieja
            foreach (Currency cr in tableA)
            {
                foreach (Rate c in cr.rates)
                {
                    if (c.effectiveDate.ToString("yyyy-MM-dd").Equals(text))
                    {
                        exist = true;
                    }
                }
            }
            if (!exist) this.ViewModel.Exist = "Brak Danych";
            else this.ViewModel.Exist = "";
        }

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

            dt.Columns.Add("Dzien", typeof(int));
            dt.Columns.Add("Nazwa Waluty", typeof(string));
            dt.Columns.Add("Skrot", typeof(string));
            dt.Columns.Add("Kurs", typeof(string));
            for (int i = 0; i < this.ViewModel.Items.Count; i++)
            {
                CurrencyView c = this.ViewModel.Items[i];
                if(c.Name == null) dt.Rows.Add(i + 1, CheckName(c.CrCode), c.CrCode, c.Mid);
                else dt.Rows.Add(i + 1, c.Name, c.CrCode, c.Mid);
            }
            return dt;
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        public string CheckName(string code)
        {
            Dictionary<string, string> appropriateName = new Dictionary<string, string>();
            appropriateName.Add("THB", "bat (Tajlandia)");
            appropriateName.Add("USD", "dolar amerykański");
            appropriateName.Add("AUD", "dolar australijski");
            appropriateName.Add("HKD", "dolar Hongkongu");
            appropriateName.Add("CAD", "dolar kanadyjski");
            appropriateName.Add("NZD", "dolar nowozelandzki");
            appropriateName.Add("SGD", "dolar singapurski");
            appropriateName.Add("EUR", "euro");
            appropriateName.Add("HUF", "forint (Węgry)");
            appropriateName.Add("CHF", "frank szwajcarski");
            appropriateName.Add("GBP", "funt szterling");
            appropriateName.Add("UAH", "hrywna (Ukraina)");
            appropriateName.Add("JPY", "jen (Japonia)");
            appropriateName.Add("CZK", "korona czeska");
            appropriateName.Add("DKK", "korona duńska");
            appropriateName.Add("ISK", "korona islandzka");
            appropriateName.Add("NOK", "korona norweska");
            appropriateName.Add("SEK", "korona szwedzka");
            appropriateName.Add("HRK", "kuna (Chorwacja)");
            appropriateName.Add("RON", "lej rumuński");
            appropriateName.Add("BGN", "lew (Bułgaria)");
            appropriateName.Add("TRY", "lira turecka");
            appropriateName.Add("ILS", "nowy izraelski szekel");
            appropriateName.Add("CLP", "peso chilijskie");
            appropriateName.Add("PHP", "peso filipińskie");
            appropriateName.Add("MXN", "peso meksykańskie");
            appropriateName.Add("ZAR", "rand (Republika Południowej Afryki)");
            appropriateName.Add("BRL", "real (Brazylia)");
            appropriateName.Add("MYR", "ringgit (Malezja)");
            appropriateName.Add("RUB", "rubel rosyjski");
            appropriateName.Add("IDR", "rupia indonezyjska");
            appropriateName.Add("INR", "rupia indyjska");
            appropriateName.Add("KRW", "won południowokoreański");
            appropriateName.Add("CNY", "yuan renminbi (Chiny)");
            appropriateName.Add("XDR", "SDR (MFW)");

            return appropriateName[code];

        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            CoreApplication.Exit();
        }

        public async void Load(string fileName)
        {
            var list = new List<CurrencyView>();
            // Check if we had previously Save information of our friends
            // previously
            try
            {
                Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile sampleFile = await storageFolder.GetFileAsync("sample.json");
                var text = await FileIO.ReadTextAsync(sampleFile);
                list = JsonConvert.DeserializeObject<List<CurrencyView>>(text);
                this.ViewModel.Items = list;
                System.Diagnostics.Debug.WriteLine("Wielkosc listy: " + list.Count);
                DataTable dt = GetDataTable();
                FillDataGrid(dt, this.dataGrid);
                dataGrid.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}

