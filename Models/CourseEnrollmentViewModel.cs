using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace PhaseOne.Models
{
    public class CourseEnrollmentViewModel
    {
        public int CourseId { get; set; }
        public int Year { get; set; }
        public string Semester { get; set; } = "Winter"; // Winter/Summer

        public List<SelectableStudent> Students { get; set; }
    }

}