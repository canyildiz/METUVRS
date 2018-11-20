using System.Collections.Generic;

namespace METU.VRS.Models
{
    public class UserRole
    {
        public int ID { get; set; }
        public string UID { get; set; }
        public string Description { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}