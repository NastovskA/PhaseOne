using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhaseOne.Data;
using PhaseOne.ViewModels;

namespace PhaseOne.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProfessorDashboardController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var email = User?.Identity?.Name;
            if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

            var teacher = await _context.Teachers.AsNoTracking()
                .FirstOrDefaultAsync(t => t.Email == email);
            if (teacher == null) return Forbid();

            var myCourses = await _context.Courses.AsNoTracking()
                .Where(c => c.FirstTeacherId == teacher.Id || c.SecondTeacherId == teacher.Id)
                .OrderBy(c => c.Semester).ThenBy(c => c.Title)
                .ToListAsync();

            return View(myCourses);
        }

        public async Task<IActionResult> Course(int id, int? year)
        {
            var email = User?.Identity?.Name;
            if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

            var teacher = await _context.Teachers.AsNoTracking()
                .FirstOrDefaultAsync(t => t.Email == email);
            if (teacher == null) return Forbid();

            var course = await _context.Courses.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (course == null) return NotFound();

            if (course.FirstTeacherId != teacher.Id && course.SecondTeacherId != teacher.Id)
                return Forbid();

            var years = await _context.Enrollments
                .Where(e => e.CourseId == id && e.Year > 0)
                .Select(e => e.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            var selectedYear = year ?? years.FirstOrDefault();

            var enrollments = await _context.Enrollments
                .Where(e => e.CourseId == id && e.Year == selectedYear)
                .Include(e => e.Student)
                .OrderBy(e => e.Student.LastName).ThenBy(e => e.Student.FirstName)
                .Select(e => new ProfessorEnrollmentRowVm
                {
                    EnrollmentId = e.Id,
                    StudentIndex = e.Student.StudentId,
                    StudentName = e.Student.FirstName + " " + e.Student.LastName,

                    ExamPoints = e.ExamPoints,
                    SeminalPoints = e.SeminalPoints,
                    ProjectPoints = e.ProjectPoints,
                    AdditionalPoints = e.AdditionalPoints,

                    Grade = e.Grade,
                    FinishDate = e.FinishDate,
                    IsActive = e.FinishDate == null,

                    // ✅ URL read-only за приказ
                    SeminalUrl = e.SeminalUrl,
                    ProjectUrl = e.ProjectUrl
                })
                .ToListAsync();

            var vm = new ProfessorCourseStudentsVm
            {
                CourseId = course.Id,
                CourseTitle = course.Title,
                AvailableYears = years,
                SelectedYear = selectedYear,
                Enrollments = enrollments
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEnrollment(ProfessorEditEnrollmentVm vm)
        {
            var email = User?.Identity?.Name;
            if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Email == email);
            if (teacher == null) return Forbid();

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == vm.EnrollmentId);

            if (enrollment == null) return NotFound();

            if (enrollment.Course.FirstTeacherId != teacher.Id &&
                enrollment.Course.SecondTeacherId != teacher.Id)
                return Forbid();

            if (enrollment.FinishDate != null)
            {
                TempData["Err"] = "Овој студент е веќе завршен и не може да се менува.";
                return RedirectToAction(nameof(Course), new { id = enrollment.CourseId, year = vm.Year });
            }

            enrollment.ExamPoints = vm.ExamPoints;
            enrollment.SeminalPoints = vm.SeminalPoints;
            enrollment.ProjectPoints = vm.ProjectPoints;
            enrollment.AdditionalPoints = vm.AdditionalPoints;
            enrollment.Grade = vm.Grade;
            enrollment.FinishDate = vm.FinishDate;

            await _context.SaveChangesAsync();

            TempData["Ok"] = "Успешно зачувано.";
            return RedirectToAction(nameof(Course), new { id = enrollment.CourseId, year = vm.Year });
        }
    }
}
