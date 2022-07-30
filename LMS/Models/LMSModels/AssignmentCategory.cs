using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class AssignmentCategory
    {
        public AssignmentCategory()
        {
            Assignments = new HashSet<Assignment>();
        }

        public int Id { get; set; }
        public int GradeWeight { get; set; }
        public int CId { get; set; }
        public string Name { get; set; } = null!;

        public virtual Class CIdNavigation { get; set; } = null!;
        public virtual ICollection<Assignment> Assignments { get; set; }
    }
}
