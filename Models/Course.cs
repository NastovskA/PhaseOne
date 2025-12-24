using PhaseOne.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhaseOne.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        [StringLength(100)]
        public string Title { get; set; } 

        [Required]
        public int Credits { get; set; } 

        [Required]
        public int Semester { get; set; } 

        [StringLength(100)]
        public string? Programme { get; set; } 

        [StringLength(25)]
        public string? EducationLevel { get; set; } 

        public int? FirstTeacherId { get; set; } 
        [ForeignKey("FirstTeacherId")]
        public virtual Teacher? FirstTeacher { get; set; }

        public int? SecondTeacherId { get; set; } 
        [ForeignKey("SecondTeacherId")]
        public virtual Teacher? SecondTeacher { get; set; }

        public virtual ICollection<Enrollment>? Enrollments { get; set; } = new List<Enrollment>();
    }
}