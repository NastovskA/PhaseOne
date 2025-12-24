using PhaseOne.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Enrollment
{
    [Key]
    public long Id { get; set; } 

    [Required]
    public int CourseId { get; set; } 
    
    [Required]
    public long StudentId { get; set; } 
    
    [StringLength(10)]
    public string? Semester { get; set; } 

    [Required]
    public int? Year { get; set; } 

    public int? Grade { get; set; } 

    [StringLength(255)]
    public string? SeminalUrl { get; set; } 

    [StringLength(255)]
    public string? ProjectUrl { get; set; } 

    public int? ExamPoints { get; set; } 

    public int? SeminalPoints { get; set; } 

    public int? ProjectPoints { get; set; } 

    public int? AdditionalPoints { get; set; } 

    [DataType(DataType.Date)]
    public DateTime? FinishDate { get; set; }

    public Course Course { get; set; } = null!;
    public Student Student { get; set; } = null!;


}