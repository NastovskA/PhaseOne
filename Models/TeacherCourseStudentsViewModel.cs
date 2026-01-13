namespace PhaseOne.Models
{
    public class TeacherCourseStudentsViewModel
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = "";
        public int SelectedYear { get; set; }
        public List<int> AvailableYears { get; set; } = new();
        public List<TeacherEnrollmentRow> Enrollments { get; set; } = new();
    }

    public class TeacherEnrollmentRow
    {
        public long EnrollmentId { get; set; }
        public string StudentDisplay { get; set; } = "";
        public string Semester { get; set; } = "";

        public int? ExamPoints { get; set; }
        public int? SeminalPoints { get; set; }
        public int? ProjectPoints { get; set; }
        public int? AdditionalPoints { get; set; }
        public int? Grade { get; set; }
        public DateTime? FinishDate { get; set; }

        public string? SeminalUrl { get; set; }
        public string? ProjectUrl { get; set; }

        public bool IsActive { get; set; }

    }
}
