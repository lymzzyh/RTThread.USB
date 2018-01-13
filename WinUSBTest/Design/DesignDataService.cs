using System;
using WinUSBTest.Model;

namespace WinUSBTest.Design
{
    public class DesignDataService : IDataService
    {
        public void GetData(Action<DataItem, Exception> callback)
        {
            // Use this to create design time data

            var item = new DataItem();
            callback(item, null);
        }
    }
}