using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Google.Protobuf;
using net.atos.daf.ct2.termsandconditions.entity;
using net.atos.daf.ct2.translation.entity;

namespace net.atos.daf.ct2.translationservice.Entity
{
    public class Mapper
    {


        public Translationupload ToTranslationUploadEntity(TranslationUploadRequest request)
        {
            Translationupload obj = new Translationupload();
            // obj.id = request.Id;


            obj.Translationss = new List<Translations>();
            obj.FileName = request.FileName;
            obj.Description = request.Description;
            obj.FileSize = request.FileSize;
            obj.FailureCount = request.FailureCount;
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
                    obj.Translationss.Add(trans);
                }

            }
            if (request.File != null)
            {
                Encoding u8 = Encoding.UTF8;
                BinaryFormatter bf = new BinaryFormatter();
                //MemoryStream ms = new MemoryStream();
                using (MemoryStream ms = new MemoryStream())
                {
                    bf.Serialize(ms, obj.Translationss);
                    obj.File = ms.ToArray();
                }
                //bf.Serialize(ms, request.File);
                //obj.file = request.File.SelectMany(s =>
                // System.Text.Encoding.UTF8.GetBytes(s + Environment.NewLine)).ToArray();


            }

            obj.AddedCount = request.AddedCount;
            obj.UpdatedCount = request.UpdatedCount;


            return obj;
        }
        public int ToTranslationEntity(FileUploadDetailsRequest request)
        {
            Translationupload obj = new Translationupload();
            obj.Id = request.FileID;
            return obj.Id;
        }

        public TranslationUpload ToTranslationUploadDetailEntity(Translationupload translationupload)
        {
            TranslationUpload obj = new TranslationUpload();
            obj.Id = translationupload.Id;
            if (!string.IsNullOrEmpty(translationupload.FileName))
                obj.FileName = translationupload.FileName;
            if (!string.IsNullOrEmpty(translationupload.Description))
                obj.Description = translationupload.Description;
            obj.FileSize = translationupload.FileSize;
            obj.FailureCount = translationupload.FailureCount;
            // obj.CreatedBy = translationupload.created_by;
            if (translationupload.File != null)
            {
                // obj.File = translationupload.file;
                //ByteString bytestring;
                using (var str = new MemoryStream(translationupload.File))
                {
                    //obj.File = ByteString.FromStream(str);
                    var Translations = Parse(translationupload.File);
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

            obj.AddedCount = translationupload.AddedCount;
            obj.CreatedAt = translationupload.CreatedAt;
            obj.UpdatedCount = translationupload.UpdatedCount;
            return obj;
        }
        public static dynamic Parse(byte[] json)
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
            public List<Translations> Language { get; set; }
            public List<Translations> Unit { get; set; }
            public List<Translations> Timezone { get; set; }
            public List<Translations> Currency { get; set; }
            public List<Translations> LandingPageDisplay { get; set; }
            public List<Translations> DateFormat { get; set; }
            public List<Translations> TimeFormat { get; set; }
            public List<Translations> VehicleDisplay { get; set; }

        }

        public AccountTermsCondition ToAcceptedTermConditionEntity(AcceptedTermConditionRequest request)
        {
            AccountTermsCondition accountTermsCondition = new AccountTermsCondition();
            accountTermsCondition.Id = request.Id;
            accountTermsCondition.Account_Id = request.AccountId;
            accountTermsCondition.Organization_Id = request.OrganizationId;
            accountTermsCondition.Terms_And_Condition_Id = request.TermsAndConditionId;
            accountTermsCondition.Version_no = request.VersionNo;
            accountTermsCondition.Accepted_Date = DateTime.Now;
            return accountTermsCondition;
        }

        public TermConditionReponse ToTermConditionReponse(TermsAndConditions termsAndConditions)
        {
            TermConditionReponse response = new TermConditionReponse();
            response.Id = termsAndConditions.Id;
            response.Code = termsAndConditions.Code;
            response.Versionno = termsAndConditions.Version_no;
            if (termsAndConditions.Description != null)
            {
                response.Description = ByteString.CopyFrom(termsAndConditions.Description);
            }
            response.StartDate = termsAndConditions.StartDate.ToString();
            response.EndDate = termsAndConditions.EndDate.ToString();
            response.AcceptedDate = termsAndConditions.Accepted_Date.ToString();
            response.FirstName = termsAndConditions.FirstName ?? "";
            response.Lastname = termsAndConditions.Lastname ?? "";
            return response;
        }

        public AcceptedTermConditionRequest ToAcceptedTermConditionRequestEntity(AccountTermsCondition response)
        {
            AcceptedTermConditionRequest accountTermsConditionrequest = new AcceptedTermConditionRequest();
            accountTermsConditionrequest.Id = response.Id;
            accountTermsConditionrequest.AccountId = response.Account_Id;
            accountTermsConditionrequest.OrganizationId = response.Organization_Id;
            accountTermsConditionrequest.TermsAndConditionId = response.Terms_And_Condition_Id;
            accountTermsConditionrequest.VersionNo = response.Version_no;
            return accountTermsConditionrequest;
        }

    }
}
