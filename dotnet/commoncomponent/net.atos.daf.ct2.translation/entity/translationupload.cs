using System.Collections.Generic;

namespace net.atos.daf.ct2.translation.entity
{
    public class Translationupload
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public int FileSize { get; set; }
        public int FailureCount { get; set; }
        public long CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public byte[] File { get; set; }
        public int AddedCount { get; set; }
        public int UpdatedCount { get; set; }
        public List<Translations> Translationss { get; set; }

    }
}
