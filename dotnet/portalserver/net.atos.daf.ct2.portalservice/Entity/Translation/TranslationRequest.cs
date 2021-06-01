using System.ComponentModel.DataAnnotations;

namespace net.atos.daf.ct2.portalservice.Entity.Translation
{
    public class TranslationRequest
    {
        public int Id { get; set; }
        [Required]
        [RegularExpression(@"[a-zA-Z-]*$", ErrorMessage = "Numbers are not allowed in langaugecode.")]
        public string Code { get; set; }
        [Required]
        [RegularExpression(@"[a-zA-Z-]*$", ErrorMessage = "Numbers are not allowed in type.")]
        public string Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public string Filter { get; set; }
        [Required]
        public int MenuId { get; set; }
    }

    public class TranslationFileData
    {

        public string Code { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

    }





}
