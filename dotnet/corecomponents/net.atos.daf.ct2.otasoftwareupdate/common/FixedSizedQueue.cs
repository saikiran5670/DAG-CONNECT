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
        ConcurrentQueue<CampiagnData> q = new ConcurrentQueue<CampiagnData>();
        private object _lockObject = new object();

        public int Limit { get; set; }
        public void Enqueue(CampiagnData obj)
        {
            q.Enqueue(obj);
            lock (_lockObject)
            {
                while (q.Count > Limit && q.TryDequeue(out CampiagnData overflow)) ;
            }
        }

        public string Find(CampiagnData obj)
        {
            lock (_lockObject)
            {
                return q.Where(w => w.CampaignId == obj.CampaignId && w.Code == obj.Code).FirstOrDefault()?.ReleaseNotes;
            }
        }
    }
}
