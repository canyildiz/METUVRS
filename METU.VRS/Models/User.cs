using System.Collections.Generic;

namespace METU.VRS.Models
{
    public class User
    {
        public int ID { get; set; }

        public string UID { get; set; }

        public string Name { get; set; }


        public virtual ICollection<UserRole> Roles { get; set; }

        public virtual UserCategory Category { get; set; }

        public virtual BranchAffiliate Division { get; set; }
    }
}