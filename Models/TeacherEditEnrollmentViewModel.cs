using System.ComponentModel.DataAnnotations;

namespace PhaseOne.Models
{
    public class TeacherEditEnrollmentViewModel
    {
        public long EnrollmentId { get; set; }
        public int CourseId { get; set; }
        public int Year { get; set; }
        public string Semester { get; set; } = "";
        public long StudentId { get; set; }

        // editable
        public int? ExamPoints { get; set; }
        public int? SeminalPoints { get; set; }
        public int? ProjectPoints { get; set; }
        public int? AdditionalPoints { get; set; }
        public int? Grade { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FinishDate { get; set; }

        // read-only
        public string? SeminalUrl { get; set; }
        public string? ProjectUrl { get; set; }

        public bool IsActive { get; set; }
    }
}
