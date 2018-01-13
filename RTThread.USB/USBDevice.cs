
namespace RTThread.USB
{
    public class USBDevice
    {
        private string path;
        private string name;
        public USBDevice()
        {

        }
        public USBDevice(string path)
        {
            this.path = path;
        }
        public USBDevice(string path, string name)
        {
            this.path = path;
            this.name = name;
        }

        public string Path { get => path; set => path = value; }
        public string Name { get => name; set => name = value; }
    }
}
