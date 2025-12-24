//Ova mi pretstavuva glavno povrzuvanje pomegu C# kodot i bazata na podatoci vo aplikacijata.
// prebaruvam podatoci
//skladiram
//OD OVDE KE KOMUNICIRAM SO TABELATA


using PhaseOne.Models;
using Microsoft.EntityFrameworkCore;

namespace PhaseOne.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Ова му кажува на Entity Framework дека овие класи треба да бидат табели во SQL
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        // ---  ДЕФИНИРАЊЕ НА РЕЛАЦИИТЕ (OnModelCreating) ---
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Конфигурација за Првиот Наставник на предметот
            modelBuilder.Entity<Course>()
                .HasOne(c => c.FirstTeacher)           // Предметот има еден FirstTeacher
                .WithMany(t => t.FirstTeacherCourses)  // Наставникот може да биде "прв" на многу предмети
                .HasForeignKey(c => c.FirstTeacherId)  // Клучот е FirstTeacherId
                .OnDelete(DeleteBehavior.Restrict);    // Ако се избрише наставникот, да не се избрише предметот автоматски

            // 2. Конфигурација за Вториот Наставник на предметот
            modelBuilder.Entity<Course>()
                .HasOne(c => c.SecondTeacher)          // Предметот има еден SecondTeacher
                .WithMany(t => t.SecondTeacherCourses) // Наставникот може да биде "втор" на многу предмети
                .HasForeignKey(c => c.SecondTeacherId) // Клучот е SecondTeacherId
                .OnDelete(DeleteBehavior.Restrict);

            // 3. Many-to-Many релација (Enrollment)
            // Поврзување на Enrollment со Student
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId);

            // Поврзување на Enrollment со Course
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId);

            modelBuilder.Entity<Enrollment>()
                .HasIndex(e => new { e.CourseId, e.StudentId })
                .IsUnique();

            modelBuilder.Entity<Student>()
              .HasIndex(s => s.StudentId)
              .IsUnique();


            modelBuilder.Entity<Student>().HasData(
                new Student { Id = 1, StudentId = "2025001", FirstName = "Elena", LastName = "Kostova", EnrollmentDate = new DateTime(2022, 10, 1), AcquiredCredits = 60, CurrentSemestar = 3, EducationLevel = "Bachelor" },
                new Student { Id = 2, StudentId = "2025002", FirstName = "Ivan", LastName = "Nikolov", EnrollmentDate = new DateTime(2021, 10, 1), AcquiredCredits = 120, CurrentSemestar = 5, EducationLevel = "Bachelor" },
                new Student { Id = 3, StudentId = "2025003", FirstName = "Marija", LastName = "Stojanova", EnrollmentDate = new DateTime(2023, 10, 1), AcquiredCredits = 30, CurrentSemestar = 2, EducationLevel = "Bachelor" },
                new Student { Id = 4, StudentId = "2025004", FirstName = "Petar", LastName = "Iliev", EnrollmentDate = new DateTime(2020, 10, 1), AcquiredCredits = 180, CurrentSemestar = 7, EducationLevel = "Bachelor" },
                new Student { Id = 5, StudentId = "2025005", FirstName = "Simona", LastName = "Georgieva", EnrollmentDate = new DateTime(2022, 10, 1), AcquiredCredits = 90, CurrentSemestar = 4, EducationLevel = "Bachelor" },
                new Student { Id = 6, StudentId = "2025006", FirstName = "Aleksandar", LastName = "Trajkovski", EnrollmentDate = new DateTime(2021, 10, 1), AcquiredCredits = 150, CurrentSemestar = 6, EducationLevel = "Bachelor" },
                new Student { Id = 7, StudentId = "2025007", FirstName = "Sara", LastName = "Mitreva", EnrollmentDate = new DateTime(2023, 10, 1), AcquiredCredits = 15, CurrentSemestar = 1, EducationLevel = "Bachelor" }
            );


            modelBuilder.Entity<Teacher>().HasData(
                new Teacher { Id = 1, FirstName = "Lila", LastName = "Petrova", Degree = "PhD", AcademicRank = "Professor", OfficeNumber = "101", HireDate = new DateTime(2015, 9, 1) },
                new Teacher { Id = 2, FirstName = "Marko", LastName = "Stojanovski", Degree = "MSc", AcademicRank = "Assistant", OfficeNumber = "102", HireDate = new DateTime(2018, 2, 15) },
                new Teacher { Id = 3, FirstName = "Ivana", LastName = "Koleva", Degree = "PhD", AcademicRank = "Associate Professor", OfficeNumber = "103", HireDate = new DateTime(2016, 5, 10) },
                new Teacher { Id = 4, FirstName = "Stefan", LastName = "Jovanov", Degree = "MSc", AcademicRank = "Assistant", OfficeNumber = "104", HireDate = new DateTime(2019, 1, 20) },
                new Teacher { Id = 5, FirstName = "Biljana", LastName = "Ristova", Degree = "PhD", AcademicRank = "Professor", OfficeNumber = "105", HireDate = new DateTime(2014, 11, 5) },
                new Teacher { Id = 6, FirstName = "Miki", LastName = "Todorov", Degree = "MSc", AcademicRank = "Assistant", OfficeNumber = "106", HireDate = new DateTime(2020, 3, 12) },
                new Teacher { Id = 7, FirstName = "Dragana", LastName = "Spasova", Degree = "PhD", AcademicRank = "Associate Professor", OfficeNumber = "107", HireDate = new DateTime(2017, 7, 7) }
            );




            modelBuilder.Entity<Course>().HasData(
                new Course { Id = 1, Title = "Programming 1", Credits = 6, Semester = 1, Programme = "Computer Science", EducationLevel = "Bachelor", FirstTeacherId = 1, SecondTeacherId = 2 },
                new Course { Id = 2, Title = "Databases", Credits = 6, Semester = 3, Programme = "Software Engineering", EducationLevel = "Bachelor", FirstTeacherId = 2, SecondTeacherId = 3 },
                new Course { Id = 3, Title = "Algorithms", Credits = 6, Semester = 5, Programme = "Computer Science", EducationLevel = "Bachelor", FirstTeacherId = 3, SecondTeacherId = 4 },
                new Course { Id = 4, Title = "Web Development", Credits = 6, Semester = 4, Programme = "Software Engineering", EducationLevel = "Bachelor", FirstTeacherId = 4, SecondTeacherId = 5 },
                new Course { Id = 5, Title = "Operating Systems", Credits = 6, Semester = 6, Programme = "Computer Science", EducationLevel = "Bachelor", FirstTeacherId = 5, SecondTeacherId = 6 },
                new Course { Id = 6, Title = "Networks", Credits = 6, Semester = 7, Programme = "Computer Science", EducationLevel = "Bachelor", FirstTeacherId = 6, SecondTeacherId = 7 },
                new Course { Id = 7, Title = "Software Engineering", Credits = 6, Semester = 8, Programme = "Software Engineering", EducationLevel = "Bachelor", FirstTeacherId = 7, SecondTeacherId = 1 }
            );


            modelBuilder.Entity<Enrollment>().HasData(
                new Enrollment
                {
                    Id = 1,
                    CourseId = 1,
                    StudentId = 1,
                    Semester = "winter",
                    Year = 2022,
                    Grade = 10,
                    SeminalUrl = "http://example.com/seminal1",
                    ProjectUrl = "http://example.com/project1",
                    ExamPoints = 50,
                    SeminalPoints = 20,
                    ProjectPoints = 20,
                    AdditionalPoints = 5,
                    FinishDate = new DateTime(2023, 6, 15)
                },
                new Enrollment
                {
                    Id = 2,
                    CourseId = 2,
                    StudentId = 2,
                    Semester = "winter",
                    Year = 2023,
                    Grade = 9,
                    SeminalUrl = "http://example.com/seminal2",
                    ProjectUrl = "http://example.com/project2",
                    ExamPoints = 45,
                    SeminalPoints = 15,
                    ProjectPoints = 25,
                    AdditionalPoints = 0,
                    FinishDate = new DateTime(2024, 6, 15)
                },
                new Enrollment
                {
                    Id = 3,
                    CourseId = 3,
                    StudentId = 3,
                    Semester = "summer",
                    Year = 2023,
                    Grade = 8,
                    SeminalUrl = "http://example.com/seminal3",
                    ProjectUrl = "http://example.com/project3",
                    ExamPoints = 40,
                    SeminalPoints = 20,
                    ProjectPoints = 20,
                    AdditionalPoints = 0,
                    FinishDate = new DateTime(2024, 6, 20)
                },
                new Enrollment
                {
                    Id = 4,
                    CourseId = 4,
                    StudentId = 4,
                    Semester = "summer",
                    Year = 2021,
                    Grade = 7,
                    SeminalUrl = "http://example.com/seminal4",
                    ProjectUrl = "http://example.com/project4",
                    ExamPoints = 35,
                    SeminalPoints = 15,
                    ProjectPoints = 15,
                    AdditionalPoints = 5,
                    FinishDate = new DateTime(2022, 6, 10)
                },
                new Enrollment
                {
                    Id = 5,
                    CourseId = 5,
                    StudentId = 5,
                    Semester = "winter",
                    Year = 2022,
                    Grade = 10,
                    SeminalUrl = "http://example.com/seminal5",
                    ProjectUrl = "http://example.com/project5",
                    ExamPoints = 50,
                    SeminalPoints = 20,
                    ProjectPoints = 20,
                    AdditionalPoints = 10,
                    FinishDate = new DateTime(2023, 6, 15)
                },
                new Enrollment
                {
                    Id = 6,
                    CourseId = 6,
                    StudentId = 6,
                    Semester = "summer",
                    Year = 2023,
                    Grade = 9,
                    SeminalUrl = "http://example.com/seminal6",
                    ProjectUrl = "http://example.com/project6",
                    ExamPoints = 45,
                    SeminalPoints = 15,
                    ProjectPoints = 25,
                    AdditionalPoints = 5,
                    FinishDate = new DateTime(2024, 6, 15)
                },
                new Enrollment
                {
                    Id = 7,
                    CourseId = 7,
                    StudentId = 7,
                    Semester = "winter",
                    Year = 2024,
                    Grade = 8,
                    SeminalUrl = "http://example.com/seminal7",
                    ProjectUrl = "http://example.com/project7",
                    ExamPoints = 40,
                    SeminalPoints = 20,
                    ProjectPoints = 20,
                    AdditionalPoints = 0,
                    FinishDate = new DateTime(2025, 6, 20)
                }
            );
        }
        public DbSet<PhaseOne.Models.SeminarWork> SeminarWork { get; set; } = default!;
    }
}
