using System;

namespace DAF.Entity
{
    public class UserGenralPreferences
    {
        public int UserGeneralPreferenceId { get; set; }
        public int UserorgId { get; set; }
        public int LanguageMasterId { get; set; }
        public int Timezoneid { get; set; }
        public int CurrencyId { get; set; }
        public int UnitId { get; set; }
        public int VehicleDisplayId { get; set; }
        public int DateFormatId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
