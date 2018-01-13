using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Text;
using System.Windows;
using System.Windows.Input;
using WinUSBTest.Model;

namespace WinUSBTest.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private DataItem dataItem;
        public string ConnectStatus => dataItem.IsDeviceConnect ? "已连接" : "未连接";
        public string FileName{ get; set; }
        public string FileContext { get; set; }

        public ICommand ReadFile { get; set; }
        public ICommand WriteFile { get; set; }

        public void ReadFileAction()
        {
            FileContext = Encoding.ASCII.GetString(dataItem.GetFileFromDevice(FileName));
            RaisePropertyChanged("FileContext");
        }
        public void WriteFileAction()
        {
            dataItem.WriteFileToDevice(FileName, Encoding.ASCII.GetBytes(FileContext));
            MessageBox.Show("Wite Successful!");
        }


        public MainViewModel(IDataService dataService)
        {
            dataService.GetData(
                (item, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }

                    dataItem = item;
                });
            FileName = "hello.txt";
            ReadFile = new RelayCommand(ReadFileAction);
            WriteFile = new RelayCommand(WriteFileAction);
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}