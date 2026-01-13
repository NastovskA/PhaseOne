using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PhaseOne.Models
{
    public class AdminDeactivateEnrollmentsViewModel
    {
        public int CourseId { get; set; }

        public int Year { get; set; }

        public string Semester { get; set; } = "Winter";

        [Required]
        [DataType(DataType.Date)]
        public DateTime FinishDate { get; set; }

        public List<EnrollmentDeactivateItem> Enrollments { get; set; } = new();

        public List<long> SelectedEnrollmentIds { get; set; } = new();
    }

    public class EnrollmentDeactivateItem
    {
        public long EnrollmentId { get; set; }
        public long StudentId { get; set; }
        public string StudentDisplay { get; set; } = "";
    }
}
