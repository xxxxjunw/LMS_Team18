using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class EnrollmentGrade
    {
        public string UId { get; set; } = null!;
        public int CId { get; set; }
        public string Grade { get; set; } = null!;

        public virtual Class CIdNavigation { get; set; } = null!;
        public virtual Student UIdNavigation { get; set; } = null!;
    }
}
