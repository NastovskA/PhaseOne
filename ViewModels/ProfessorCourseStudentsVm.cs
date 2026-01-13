using System.Collections.Generic;

namespace PhaseOne.ViewModels
{
    public class ProfessorCourseStudentsVm
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = "";

        public List<int> AvailableYears { get; set; } = new();
        public int SelectedYear { get; set; }

        public List<ProfessorEnrollmentRowVm> Enrollments { get; set; } = new();
    }
}
