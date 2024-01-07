using System;
using System.Collections.Generic;

namespace HomeStay.Models
{
    public partial class Role
    {
        public Role()
        {
            Accounts = new HashSet<Account>();
        }

        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
