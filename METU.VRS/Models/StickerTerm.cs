using System;

namespace METU.VRS.Models
{
    public class StickerTerm
    {
        public int ID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TermTypes Type { get; set; }

        public bool IsExpired
        {
            get
            {
                return (EndDate <= DateTime.Today);
            }
        }
    }

    public enum TermTypes
    {
        LongTerm = 10,
        CalendarYear = 20,
        AcademicYear = 30
    }
}