using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.atos.daf.ct2.otasoftwareupdate.common
{
    public class CampiagnData
    {
        public string CampaignId { get; set; }
        public string Code { get; set; }
        public string ReleaseNotes { get; set; }
    }

    public class FixedSizedQueue
    {
        private readonly ConcurrentQueue<CampiagnData> _q = new ConcurrentQueue<CampiagnData>();
        private readonly object _lockObject = new object();

        public int Limit { get; set; }
        public void Enqueue(CampiagnData obj)
        {
            _q.Enqueue(obj);
            lock (_lockObject)
            {
                while (_q.Count > Limit && _q.TryDequeue(out CampiagnData overflow)) ;
            }
        }

        public string Find(CampiagnData obj)
        {
            lock (_lockObject)
            {
                return _q.Where(w => w.CampaignId == obj.CampaignId && w.Code == obj.Code).FirstOrDefault()?.ReleaseNotes;
            }
        }
    }
}
