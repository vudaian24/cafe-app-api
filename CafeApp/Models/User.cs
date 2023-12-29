using System;
using System.Collections.Generic;

namespace CafeApp.Models
{
    public partial class User
    {
        public User()
        {
            Accounts = new HashSet<Account>();
        }

        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Avatar { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
