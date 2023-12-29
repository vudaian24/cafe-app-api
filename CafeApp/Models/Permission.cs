using System;
using System.Collections.Generic;

namespace CafeApp.Models
{
    public partial class Permission
    {
        public Permission()
        {
            Accounts = new HashSet<Account>();
        }

        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
