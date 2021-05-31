using System;

namespace net.atos.daf.ct2.tcucore
{
    public class TCU
    {
        private String id;
        private String brand;
        private String version;

        public TCU()
        {
        }

        public TCU(string _id, string _brand, string _version)
        {
            this.id = _id;
            this.brand = _brand;
            this.version = _version;
        }

        public string ID
        {
            get => id;
            set => id = value;
        }

        public string Brand
        {
            get => brand;
            set => brand = value;
        }

        public string Version
        {
            get => version;
            set => version = value;
        }
    }
}
