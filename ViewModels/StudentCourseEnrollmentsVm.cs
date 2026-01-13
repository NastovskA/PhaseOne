using System.Collections.Generic;
using PhaseOne.Models;

namespace PhaseOne.ViewModels
{
    public class StudentCourseEnrollmentsVm
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = "";
        public List<int> AvailableYears { get; set; } = new();
        public int SelectedYear { get; set; }
        public List<Enrollment> Enrollments { get; set; } = new();
    }
}
