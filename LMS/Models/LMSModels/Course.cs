using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Course
    {
        public Course()
        {
            Classes = new HashSet<Class>();
        }

        public uint CatalogId { get; set; }
        public uint CourseNum { get; set; }
        public string? Name { get; set; }
        public string? Subject { get; set; }

        public virtual Department? SubjectNavigation { get; set; }
        public virtual ICollection<Class> Classes { get; set; }
    }
}
