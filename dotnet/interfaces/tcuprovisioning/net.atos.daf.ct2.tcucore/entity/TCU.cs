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

        public TCU(string id, string brand, string version)
        {
            this.ID = id;
            this.Brand = brand;
            this.Version = version;
        }
    }
}
