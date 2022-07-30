using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Assignment
    {
        public Assignment()
        {
            Submissions = new HashSet<Submission>();
        }

        public int AId { get; set; }
        public int CId { get; set; }
        public string Name { get; set; } = null!;
        public string? Contents { get; set; }
        public DateTime Due { get; set; }
        public int? Points { get; set; }

        public virtual AssignmentCategory CIdNavigation { get; set; } = null!;
        public virtual ICollection<Submission> Submissions { get; set; }
    }
}
