using System;

namespace PhaseOne.ViewModels
{
    public class ProfessorEnrollmentRowVm
    {
        public long EnrollmentId { get; set; }

        public string StudentIndex { get; set; } = "";
        public string StudentName { get; set; } = "";

        public int? ExamPoints { get; set; }
        public int? SeminalPoints { get; set; }
        public int? ProjectPoints { get; set; }
        public int? AdditionalPoints { get; set; }

        public int? Grade { get; set; }
        public DateTime? FinishDate { get; set; }

        public bool IsActive { get; set; }

        public string? SeminalUrl { get; set; }
        public string? ProjectUrl { get; set; }

    }
}
