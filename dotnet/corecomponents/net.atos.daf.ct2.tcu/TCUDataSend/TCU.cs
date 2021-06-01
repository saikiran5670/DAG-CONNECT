using System;

namespace TCUSend
{
    public class TCU
    {
        private String id;
        private String brand;
        private String version;

        public TCU()
        {

        }

        public TCU(string ID, string Brand, string Version)
        {
            this.id = ID;
            this.brand = Brand;
            this.version = Version;
        }

        public string ID { get => id; set => id = value; }

        public string Brand { get => brand; set => brand = value; }

        public string Version { get => version; set => version = value; }
    }
}
