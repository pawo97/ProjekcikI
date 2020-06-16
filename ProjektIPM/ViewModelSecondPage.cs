using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ProjektIPM
{
    public class ViewModelSecondPage : INotifyPropertyChanged
    {
        Windows.Storage.ApplicationDataContainer localSettings;
        Windows.Storage.ApplicationDataCompositeValue composite;
        private static ViewModelSecondPage myInstance;

        private string firstDate;
        private string secondDate;
        private int progress = 0;
        private int start = 0;
        private int finish = 20;
        private string status;

        private List<Rate> itemsChart = new List<Rate>();

        public List<Rate> ItemsChart
        {
            get
            {
                return this.itemsChart;
            }
            set
            {
                this.itemsChart = value;
                //this.OnPropertyChanged();
                this.StoreLocalSettings();
            }
        }

        public ViewModelSecondPage()
        {
            myInstance = this;
            localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            composite = (Windows.Storage.ApplicationDataCompositeValue)localSettings.Values["DataBindingViewModel2"];
            if (composite == null)
            {
                composite = new Windows.Storage.ApplicationDataCompositeValue();
                StoreLocalSettings();
            }
            else
            {
                firstDate = (string)composite["firstDate"];
                secondDate = (string)composite["secondDate"];
                progress = (int)composite["progress"];
                start = (int)composite["start"];
                finish = (int)composite["finish"];
                status = (string)composite["status"];
            }
        }


        public string Status
        {
            get
            {
                return $"{this.status}";
            }
            set
            {
                this.status = value;
                this.OnPropertyChanged();
                StoreLocalSettings();
            }
        }

        public int Progress
        {
            get
            {
                return this.progress;
            }
            set
            {
                this.progress = value;
                this.OnPropertyChanged();
                StoreLocalSettings();
            }
        }

        public int Start
        {
            get
            {
                return this.start;
            }
            set
            {
                this.start = value;
                this.OnPropertyChanged();
                StoreLocalSettings();
            }
        }

        public int Finish
        {
            get
            {
                return this.finish;
            }
            set
            {
                this.finish = value;
                this.OnPropertyChanged();
                StoreLocalSettings();
            }
        }

        public string FirstDate
        {
            get
            {
                return $"{this.firstDate}";
            }
            set
            {
                this.firstDate = value;
                this.OnPropertyChanged();
                StoreLocalSettings();
            }
        }

        public string SecondDate
        {
            get
            {
                return $"{this.secondDate}";
            }
            set
            {
                this.secondDate = value;
                this.OnPropertyChanged();
                StoreLocalSettings();
            }
        }


        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void StoreLocalSettings()
        {
            composite["firstDate"] = firstDate;
            composite["secondDate"] = secondDate;
            composite["progress"] = progress;
            composite["start"] = start;
            composite["status"] = status;
            composite["finish"] = finish;
            Save("listOfCurrencies2.JSON", itemsChart);
            localSettings.Values["DataBindingViewModel2"] = composite;
        }

        public async void Save(string fileName, List<Rate> list)
        {
            try
            {
                Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile sampleFile = await storageFolder.CreateFileAsync("sampleChart.json", Windows.Storage.CreationCollisionOption.ReplaceExisting);
                await Windows.Storage.FileIO.WriteTextAsync(sampleFile, JsonConvert.SerializeObject(list));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}
