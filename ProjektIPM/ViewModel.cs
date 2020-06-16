using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProjektIPM;
using Windows.Storage;

namespace ProjektIPM
{
    public class ViewModel : INotifyPropertyChanged
    {
        Windows.Storage.ApplicationDataContainer localSettings;
        Windows.Storage.ApplicationDataCompositeValue composite;
        private static ViewModel myInstance;

        private string currence;
        private string exchanger;
        private string dateOfPublication;
        private string exist;
        private List<CurrencyView> items = new List<CurrencyView>();

        public ViewModel()
        {
            myInstance = this;
            localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            composite = (Windows.Storage.ApplicationDataCompositeValue)localSettings.Values["DataBindingViewModel"];
            if (composite == null)
            {
                composite = new Windows.Storage.ApplicationDataCompositeValue();
                StoreLocalSettings();
            }
            else
            {
                currence = (string)composite["currence"];
                exchanger = (string)composite["exchanger"];
                dateOfPublication = (string)composite["dateOfPublication"];
                exist = (string)composite["exist"];
                System.Diagnostics.Debug.WriteLine("Wielkosc listy czy wczytao: " + Items.Count);
            }
        }

        public List<CurrencyView> Items
        {
            get
            {
                return this.items;
            }
            set
            {
                this.items = value;
                //this.OnPropertyChanged();
                this.StoreLocalSettings();
            }
        }

        public string Currence
        {
            get
            {
                return $"{this.currence}";
            }
            set
            {
                this.currence = value;
                this.OnPropertyChanged();
                this.StoreLocalSettings();
            }
        }

        public string Exchanger
        {
            get
            {
                return $"{this.exchanger}";
            }
            set
            {
                this.exchanger = value;
                this.OnPropertyChanged();
                this.StoreLocalSettings();
            }
        }

        public string DateOfPublication
        {
            get
            {
                return $"Data publikacji: {this.dateOfPublication}";
            }
            set
            {
                this.dateOfPublication = value;
                this.OnPropertyChanged();
                this.StoreLocalSettings();
            }
        }

        public string Exist
        {
            get
            {
                return $"{this.exist}";
            }
            set
            {
                this.exist = value;
                this.OnPropertyChanged();
                this.StoreLocalSettings();
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void StoreLocalSettings()
        {
            composite["currence"] = currence;
            composite["exchanger"] = exchanger;
            composite["dateOfPublication"] = dateOfPublication;
            composite["exist"] = exist;
            Save("listOfCurrencies.JSON", items);
            localSettings.Values["DataBindingViewModel"] = composite;
        }
        public async void Save(string fileName, List<CurrencyView> list)
        {
            try
            {
                //System.Diagnostics.Debug.WriteLine("NIE UDAO SIE" + System.IO.Path.GetDirectoryName(Windows.Storage.ApplicationData.Current.LocalFolder.Path).ToString());
                //StorageFile sampleFile = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                //await Windows.Storage.FileIO.WriteTextAsync(sampleFile, JsonConvert.SerializeObject(list));
                Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile sampleFile = await storageFolder.CreateFileAsync("sample.json", Windows.Storage.CreationCollisionOption.ReplaceExisting);
                await Windows.Storage.FileIO.WriteTextAsync(sampleFile, JsonConvert.SerializeObject(list));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

       

    }
}

