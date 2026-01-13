using System;
using System.ComponentModel.DataAnnotations;

namespace PhaseOne.ViewModels
{
    public class ProfessorEditEnrollmentVm
    {
        public long EnrollmentId { get; set; }
        public int CourseId { get; set; }
        public int Year { get; set; } // за да се врати на истата година после save

        [Range(0, 100)]
        public int? ExamPoints { get; set; }

        [Range(0, 100)]
        public int? SeminalPoints { get; set; }

        [Range(0, 100)]
        public int? ProjectPoints { get; set; }

        [Range(0, 100)]
        public int? AdditionalPoints { get; set; }

        [Range(5, 10)]
        public int? Grade { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FinishDate { get; set; }
    }
}
