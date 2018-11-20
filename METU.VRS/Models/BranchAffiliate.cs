namespace METU.VRS.Models
{
    public class BranchAffiliate
    {
        public int ID { get; set; }
        public string UID { get; set; }
        public string Name { get; set; }
        public BranchAffiliateTypes Type { get; set; }
    }
}

namespace METU.VRS.Models
{
    public enum BranchAffiliateTypes
    {
        Faculty = 10,
        GraduateSchool = 20,
        AdministrativeUnit = 30,
        Affiliate = 100
    }
}