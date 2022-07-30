using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Student
    {
        public Student()
        {
            EnrollmentGrades = new HashSet<EnrollmentGrade>();
            Submissions = new HashSet<Submission>();
        }

        public string UId { get; set; } = null!;
        public string Major { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Subject { get; set; } = null!;

        public virtual Department SubjectNavigation { get; set; } = null!;
        public virtual User UIdNavigation { get; set; } = null!;
        public virtual ICollection<EnrollmentGrade> EnrollmentGrades { get; set; }
        public virtual ICollection<Submission> Submissions { get; set; }
    }
}
