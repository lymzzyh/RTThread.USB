using System;

namespace WinUSBTest.Model
{
    public class DataService : IDataService
    {
        DataItem item;
        public DataService()
        {
            item = new DataItem();
        }
        public void GetData(Action<DataItem, Exception> callback)
        {
            callback(item, null);
        }
    }
}