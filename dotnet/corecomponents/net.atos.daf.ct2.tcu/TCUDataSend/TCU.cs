using System;

namespace TCUSend
{
    public class TCU
    {
        public TCU()
        {

        }

        public TCU(string id, string brand, string version)
        {
            ID = id;
            Brand = brand;
            Version = version;
        }

        public string ID { get;  }

        public string Brand { get;  }

        public string Version { get;  }
    }
}
