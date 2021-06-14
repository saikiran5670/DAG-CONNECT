using System;

namespace net.atos.daf.ct2.tcucore
{
    public class TCU
    {
        public string ID { get; }

        public string Brand { get; }

        public string Version { get; }
        public TCU()
        {
        }

        public TCU(string _id, string _brand, string _version)
        {
            this.ID = _id;
            this.Brand = _brand;
            this.Version = _version;
        }


    }
}
