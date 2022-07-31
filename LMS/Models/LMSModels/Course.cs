using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Course
    {
        public uint CourseNum { get; set; }
        public string? Name { get; set; }
        public string? Subject { get; set; }
        public int CatalogId { get; set; }

        public virtual Department? SubjectNavigation { get; set; }
    }
}
