using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PhaseOne.Models
{
    public class AdminEnrollViewModel
    {
        [Required]
        public int CourseId { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public string Semester { get; set; } = "Winter";

        public List<long> SelectedStudentIds { get; set; } = new();

        public List<StudentCheckboxItem> Students { get; set; } = new();
    }

    public class StudentCheckboxItem
    {
        public long Id { get; set; }
        public string FullName { get; set; } = "";
        public bool IsSelected { get; set; }
    }
}
