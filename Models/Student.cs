using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhaseOne.Models
{
    public class Student
    {
        // Примарен клуч со Identity ознака
        [Key]
        public long Id { get; set; } // bigint

        [Required]
        [StringLength(10)]
        public string StudentId { get; set; } // nvarchar(10), Nullable: No

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } // nvarchar(50), Nullable: No

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } 

        [DataType(DataType.Date)]
        public DateTime? EnrollmentDate { get; set; } 

        public int? AcquiredCredits { get; set; } 

        public int? CurrentSemestar { get; set; } 

        [StringLength(255)]
        public string? EducationLevel { get; set; } 

        //Dodavanje slika na profil
        public string? ProfileImagePath { get; set; }
        
        [Required, EmailAddress] 
        public string Email { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        // Релација: Еден студент може да има повеќе запишувања (Enrollments)
        public ICollection<Enrollment>? Enrollments { get; set; } = new List<Enrollment>();




    }
}
