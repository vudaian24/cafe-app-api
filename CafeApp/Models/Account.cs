using System;
using System.Collections.Generic;

namespace CafeApp.Models
{
    public partial class Account
    {
        public string Id { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string PermissionId { get; set; } = null!;

        public virtual Permission Permission { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
