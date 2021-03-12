using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.translation.entity;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;
using Google.Protobuf;

namespace net.atos.daf.ct2.translationservice.Entity
{
    public class Mapper
    {


        public Translationupload ToTranslationUploadEntity(TranslationUploadRequest request)
        {
            Translationupload obj = new Translationupload();
            // obj.id = request.Id;


            obj.translations = new List<Translations>();
            obj.file_name = request.FileName;
            obj.description = request.Description;
            obj.file_size = request.FileSize;
            obj.failure_count = request.FailureCount;
           // obj.created_by = request.CreatedBy;

            if (request.File !=null)
            {
                Encoding u8 = Encoding.UTF8;
                obj.file = request.File.SelectMany(s =>
 System.Text.Encoding.UTF8.GetBytes(s + Environment.NewLine)).ToArray();
             
            }

            obj.added_count = request.AddedCount;
            obj.updated_count = request.UpdatedCount;
             if (request.File !=null)
            {
                foreach (var item in request.File)
                {
                    var trans = new Translations();
                    trans.Code = item.Code;
                    trans.Type = item.Type;
                    trans.Name = item.Name;
                    trans.Value = item.Value;
                    obj.translations.Add(trans);
                }

            }
            
            return obj;
        }
        public int ToTranslationEntity(FileUploadDetailsRequest request)
        {
            Translationupload obj = new Translationupload();
            obj.id = request.FileID;
            return obj.id;
        }

        public TranslationUpload ToTranslationUploadDetailEntity(Translationupload translationupload)
        {
            TranslationUpload obj = new TranslationUpload();
            obj.Id = translationupload.id;
            if (!string.IsNullOrEmpty(translationupload.file_name))
                obj.FileName = translationupload.file_name;
            if (!string.IsNullOrEmpty(translationupload.description))
                obj.Description = translationupload.description;
            obj.FileSize = translationupload.file_size;
            obj.FailureCount = translationupload.failure_count;
           // obj.CreatedBy = translationupload.created_by;
            if(translationupload.file !=null)
            {
               // obj.File = translationupload.file;
               //ByteString bytestring;
                using (var str = new MemoryStream(translationupload.file))
                {
                    obj.File = ByteString.FromStream(str);
                }

            }

            obj.AddedCount = translationupload.added_count;
            obj.UpdatedCount = translationupload.updated_count;
            return obj;
        }

        public class Preferences
        {
            public List<Translations> language { get; set; }
            public List<Translations> unit { get; set; }
            public List<Translations> timezone { get; set; }
            public List<Translations> currency { get; set; }
            public List<Translations> landingpagedisplay { get; set; }
            public List<Translations> dateformat { get; set; }
            public List<Translations> timeformat { get; set; }
            public List<Translations> vehicledisplay { get; set; }

        }


    }
}
