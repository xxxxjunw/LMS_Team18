using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Class
    {
        public Class()
        {
            AssignmentCategories = new HashSet<AssignmentCategory>();
            EnrollmentGrades = new HashSet<EnrollmentGrade>();
        }

        public int CId { get; set; }
        public uint CatalogId { get; set; }
        public uint CourseNum { get; set; }
        public string Semester { get; set; } = null!;
        public string Location { get; set; } = null!;
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? ProfessorId { get; set; }
        public uint? Year { get; set; }

        public virtual Course C { get; set; } = null!;
        public virtual Professor? Professor { get; set; }
        public virtual ICollection<AssignmentCategory> AssignmentCategories { get; set; }
        public virtual ICollection<EnrollmentGrade> EnrollmentGrades { get; set; }
    }
}
