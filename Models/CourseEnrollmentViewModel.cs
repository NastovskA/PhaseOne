using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace PhaseOne.Models
{
    public class CourseEnrollmentViewModel
    {
        public int CourseId { get; set; } 
        public List<SelectableStudent> Students { get; set; }
    }

}