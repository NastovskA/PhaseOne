using Microsoft.AspNetCore.Http;

namespace PhaseOne.ViewModels
{
    public class StudentEditLinksVm
    {
        public long EnrollmentId { get; set; }

        public string CourseTitle { get; set; } = "";
        public int Year { get; set; }

        // read-only display
        public string? CurrentSeminalUrl { get; set; }

        // editable
        public IFormFile? SeminarFile { get; set; }
        public string? ProjectUrl { get; set; }
    }
}
