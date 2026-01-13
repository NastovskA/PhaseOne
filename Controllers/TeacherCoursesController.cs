using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhaseOne.Data;
using PhaseOne.Models;

namespace PhaseOne.Controllers
{
    [Authorize(Roles = "Professor")]
    public class TeacherCoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TeacherCoursesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1) Листа на предмети што ги предава teacher-от
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.TeacherId == null)
                return Forbid(); // нема врска со Teacher

            int teacherId = user.TeacherId.Value;

            var courses = await _context.Courses
                .Include(c => c.FirstTeacher)
                .Include(c => c.SecondTeacher)
                .Where(c => c.FirstTeacherId == teacherId || c.SecondTeacherId == teacherId)
                .OrderBy(c => c.Semester).ThenBy(c => c.Title)
                .ToListAsync();

            return View(courses);
        }

        // 2) Преглед на запишани студенти за избран курс и година
        public async Task<IActionResult> Students(int courseId, int? year = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.TeacherId == null) return Forbid();

            int teacherId = user.TeacherId.Value;

            // курсот мора да е "негов"
            var course = await _context.Courses
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == courseId && (c.FirstTeacherId == teacherId || c.SecondTeacherId == teacherId));

            if (course == null) return NotFound();

            // најди последна година ако не е зададена
            if (!year.HasValue)
            {
                year = await _context.Enrollments
                    .Where(e => e.CourseId == courseId)
                    .MaxAsync(e => (int?)e.Year) ?? DateTime.Now.Year;

                if (!year.HasValue)
                    year = DateTime.Now.Year; // ако нема ниеден запис со година
            }

            // години што постојат за dropdown
            var years = await _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Select(e => e.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            // enrollments за таа година (и студентите)
            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Where(e => e.CourseId == courseId && e.Year == year)
                .OrderBy(e => e.Student.LastName).ThenBy(e => e.Student.FirstName)
                .ToListAsync();

            var vm = new TeacherCourseStudentsViewModel
            {
                CourseId = courseId,
                CourseTitle = course.Title,
                SelectedYear = year.Value,
                AvailableYears = years,
                Enrollments = enrollments.Select(e => new TeacherEnrollmentRow
                {
                    EnrollmentId = e.Id,
                    StudentDisplay = $"{e.Student.StudentId} - {e.Student.FirstName} {e.Student.LastName}",
                    Semester = e.Semester ?? "",
                    ExamPoints = e.ExamPoints,
                    SeminalPoints = e.SeminalPoints,
                    ProjectPoints = e.ProjectPoints,
                    AdditionalPoints = e.AdditionalPoints,
                    Grade = e.Grade,
                    FinishDate = e.FinishDate,
                    SeminalUrl = e.SeminalUrl,
                    ProjectUrl = e.ProjectUrl,
                    IsActive = e.FinishDate == null
                }).ToList()
            };

            return View(vm);
        }

        // 3) Teacher менува поени/оценка/finishDate (само ако е активен)
        [HttpGet]
        public async Task<IActionResult> EditEnrollment(long id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.TeacherId == null) return Forbid();
            int teacherId = user.TeacherId.Value;

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enrollment == null) return NotFound();

            // провери курсот да е негов
            var course = await _context.Courses.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == enrollment.CourseId && (c.FirstTeacherId == teacherId || c.SecondTeacherId == teacherId));
            if (course == null) return Forbid();

            var vm = new TeacherEditEnrollmentViewModel
            {
                EnrollmentId = enrollment.Id,
                CourseId = enrollment.CourseId,
                Year = enrollment.Year != 0 ? enrollment.Year : DateTime.Now.Year,
                Semester = enrollment.Semester ?? "",
                StudentId = enrollment.StudentId,
                // editable
                ExamPoints = enrollment.ExamPoints,
                SeminalPoints = enrollment.SeminalPoints,
                ProjectPoints = enrollment.ProjectPoints,
                AdditionalPoints = enrollment.AdditionalPoints,
                Grade = enrollment.Grade,
                FinishDate = enrollment.FinishDate,
                // read-only
                SeminalUrl = enrollment.SeminalUrl,
                ProjectUrl = enrollment.ProjectUrl,
                IsActive = enrollment.FinishDate == null
            };

            // ако не е активен, ќе прикажеме read-only page
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEnrollment(TeacherEditEnrollmentViewModel vm)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.TeacherId == null) return Forbid();
            int teacherId = user.TeacherId.Value;

            var enrollment = await _context.Enrollments.FirstOrDefaultAsync(e => e.Id == vm.EnrollmentId);
            if (enrollment == null) return NotFound();

            // провери курсот да е негов
            var course = await _context.Courses.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == enrollment.CourseId && (c.FirstTeacherId == teacherId || c.SecondTeacherId == teacherId));
            if (course == null) return Forbid();

            // само активни може да се менуваат
            if (enrollment.FinishDate != null)
            {
                ModelState.AddModelError("", "This enrollment is finished and cannot be edited.");
                vm.IsActive = false;
                return View(vm);
            }

            // update allowed fields
            enrollment.ExamPoints = vm.ExamPoints;
            enrollment.SeminalPoints = vm.SeminalPoints;
            enrollment.ProjectPoints = vm.ProjectPoints;
            enrollment.AdditionalPoints = vm.AdditionalPoints;
            enrollment.Grade = vm.Grade;
            enrollment.FinishDate = vm.FinishDate;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Students), new { courseId = enrollment.CourseId, year = enrollment.Year });
        }
    }
}
