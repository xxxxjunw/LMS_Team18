using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Administrator
    {
        public string UId { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public virtual User UIdNavigation { get; set; } = null!;
    }
}
