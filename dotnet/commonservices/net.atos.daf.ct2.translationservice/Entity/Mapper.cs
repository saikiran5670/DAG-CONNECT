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
using Newtonsoft.Json;

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
            if (request.File != null)
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
            if (request.File !=null)
            {
                Encoding u8 = Encoding.UTF8;
                BinaryFormatter bf = new BinaryFormatter();
                //MemoryStream ms = new MemoryStream();
                using (MemoryStream ms = new MemoryStream())
                {
                    bf.Serialize(ms, obj.translations);
                    obj.file = ms.ToArray();
                }
                //bf.Serialize(ms, request.File);
                //obj.file = request.File.SelectMany(s =>
                // System.Text.Encoding.UTF8.GetBytes(s + Environment.NewLine)).ToArray();
                
             
            }

            obj.added_count = request.AddedCount;
            obj.updated_count = request.UpdatedCount;
             
            
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
                    //obj.File = ByteString.FromStream(str);
                    var Translations = parse(translationupload.file);
                    TranslationData objdata = new TranslationData();
                    foreach (var item in Translations)
                    {
                        objdata.Code = item.Code;
                        objdata.Type = item.Type;
                        objdata.Name = item.Name;
                        objdata.Value = item.Value;
                        obj.File.Add(objdata);
                    }
                }

            }

            obj.AddedCount = translationupload.added_count;
            obj.CreatedAt = translationupload.created_at;
            obj.UpdatedCount = translationupload.updated_count;
            return obj;
        }
        public static dynamic parse(byte[] json)
        {
            //var reader = new StreamReader(new MemoryStream(json), Encoding.Default);

            //var values = new JsonSerializer().Deserialize(new JsonTextReader(reader));
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(json, 0, json.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = (Object)binForm.Deserialize(memStream);

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
