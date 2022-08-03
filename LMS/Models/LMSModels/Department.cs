using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Department
    {
        public Department()
        {
            Professors = new HashSet<Professor>();
            Students = new HashSet<Student>();
        }

        public string Subject { get; set; } = null!;
        public string Name { get; set; } = null!;

        public virtual ICollection<Professor> Professors { get; set; }
        public virtual ICollection<Student> Students { get; set; }
    }
}
