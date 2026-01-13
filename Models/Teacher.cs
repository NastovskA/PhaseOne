using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhaseOne.Models
{
    public class Teacher
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(50)]
        public string? Degree { get; set; }

        [StringLength(25)]
        public string? AcademicRank { get; set; }

        [StringLength(10)]
        public string? OfficeNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime? HireDate { get; set; }

        // Dodavanje slika na profil
        public string? ProfileImagePath { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // Релации: Еден наставник може да предава повеќе предмети
        [InverseProperty("FirstTeacher")]
        public virtual ICollection<Course>? FirstTeacherCourses { get; set; } = new List<Course>();

        [InverseProperty("SecondTeacher")]
        public virtual ICollection<Course>? SecondTeacherCourses { get; set; } = new List<Course>();
    }
}
