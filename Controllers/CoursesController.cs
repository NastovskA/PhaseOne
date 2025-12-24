using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhaseOne.Data;
using PhaseOne.Models;

namespace PhaseOne.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =======================
        // GET: Courses (LIST + FILTER)
        // =======================
        public async Task<IActionResult> Index(string title, int? semester, string programme, int? teacherId)
        {
            var courses = _context.Courses
                .Include(c => c.FirstTeacher)
                .Include(c => c.SecondTeacher)
                .AsQueryable();

            // Филтрирање по наслов
            if (!string.IsNullOrEmpty(title))
                courses = courses.Where(c => c.Title.Contains(title));

            // Филтрирање по семестар
            if (semester.HasValue)
                courses = courses.Where(c => c.Semester == semester);

            // Филтрирање по програма
            if (!string.IsNullOrEmpty(programme))
                courses = courses.Where(c => c.Programme.Contains(programme));

            // Филтрирање по наставник
            if (teacherId.HasValue)
                courses = courses.Where(c => c.FirstTeacherId == teacherId || c.SecondTeacherId == teacherId);

            // Подготовка за dropdown на наставници за филтрирање
            ViewData["TeacherId"] = new SelectList(_context.Teachers
                .Select(t => new { t.Id, FullName = t.FirstName + " " + t.LastName }),
                "Id", "FullName", teacherId);

            return View(await courses.ToListAsync());
        }

        // =======================
        // GET: Courses/Details/5
        // =======================
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null) return NotFound();

        //    var course = await _context.Courses
        //        .Include(c => c.FirstTeacher)
        //        .Include(c => c.SecondTeacher)
        //        .FirstOrDefaultAsync(m => m.Id == id);

        //    if (course == null) return NotFound();

        //    return View(course);
        //}

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var course = await _context.Courses
                .Include(c => c.FirstTeacher)
                .Include(c => c.SecondTeacher)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                return NotFound();

            return View(course);
        }


        // =======================
        // GET: Courses/Create
        // =======================
        public IActionResult Create()
        {
            LoadTeachersDropDowns();
            return View();
        }

        // =======================
        // POST: Courses/Create
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            LoadTeachersDropDowns(course.FirstTeacherId, course.SecondTeacherId);
            return View(course);
        }

        // =======================
        // GET: Courses/Edit/5
        // =======================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            LoadTeachersDropDowns(course.FirstTeacherId, course.SecondTeacherId);
            return View(course);
        }

        // =======================
        // POST: Courses/Edit/5
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Course course)
        {
            if (id != course.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            LoadTeachersDropDowns(course.FirstTeacherId, course.SecondTeacherId);
            return View(course);
        }

        // =======================
        // GET: Courses/Delete/5
        // =======================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses
                .Include(c => c.FirstTeacher)
                .Include(c => c.SecondTeacher)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (course == null) return NotFound();

            return View(course);
        }

        // =======================
        // POST: Courses/Delete/5
        // =======================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
                _context.Courses.Remove(course);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // =======================
        // GET: PRIKAZ NA ZAPISANI STUDENTI
        // =======================

        public IActionResult EditEnrollments(int courseId)
        {
            var course = _context.Courses.Include(c => c.Enrollments).ThenInclude(e => e.Student)
                                         .FirstOrDefault(c => c.Id == courseId);

            var allStudents = _context.Students.ToList();
            var enrolledStudentIds = course.Enrollments.Select(e => e.StudentId).ToList();

            var viewModel = new CourseEnrollmentViewModel
            {
                CourseId = courseId,
                Students = allStudents.Select(s => new SelectableStudent
                {
                    StudentId = s.Id,
                    FullName = s.FirstName + " " + s.LastName,
                    IsEnrolled = enrolledStudentIds.Contains(s.Id)
                }).ToList()
            };

            return View(viewModel);
        }


        [HttpPost]
        public IActionResult UpdateEnrollments(int courseId, List<long> selectedStudentIds)
        {
            var existingEnrollments = _context.Enrollments.Where(e => e.CourseId == courseId).ToList();

            // Отстрани студенти кои не се во листата
            foreach (var enrollment in existingEnrollments)
            {
                if (!selectedStudentIds.Contains(enrollment.StudentId))
                    _context.Enrollments.Remove(enrollment);
            }

            // Додади нови студенти
            foreach (var studentId in selectedStudentIds)
            {
                if (!existingEnrollments.Any(e => e.StudentId == studentId))
                {
                    _context.Enrollments.Add(new Enrollment
                    {
                        CourseId = courseId,
                        StudentId = studentId,
                        Semester = "Winter", // default
                        Year = DateTime.Now.Year
                    });
                }
            }

            _context.SaveChanges();
            return RedirectToAction("Details", new { id = courseId });
        }



        //public async Task<IActionResult> ManageEnrollments(int courseid)
        //{
        //    var course = await _context.Courses
        //        .Include(c => c.Enrollments)
        //            .ThenInclude(e => e.Student)
        //        .FirstOrDefaultAsync(c => c.Id == id);

        //    if (course == null)
        //        return NotFound();

        //    var allStudents = await _context.Students.ToListAsync();

        //    var viewModel = new CourseEnrollmentViewModel
        //    {
        //        CourseId = course.Id,
        //        CourseTitle = course.Title,
        //        Enrollments = course.Enrollments.Select(e => new EnrollmentEditModel
        //        {
        //            EnrollmentId = e.Id,
        //            StudentId = e.StudentId,
        //            StudentName = e.Student.FirstName + " " + e.Student.LastName,
        //            Grade = e.Grade,
        //            ExamPoints = e.ExamPoints,
        //            SeminalPoints = e.SeminalPoints,
        //            ProjectPoints = e.ProjectPoints,
        //            AdditionalPoints = e.AdditionalPoints,
        //            SeminalUrl = e.SeminalUrl,
        //            ProjectUrl = e.ProjectUrl,
        //            FinishDate = e.FinishDate
        //        }).ToList(),
        //        AvailableStudents = allStudents
        //            .Where(s => !course.Enrollments.Any(e => e.StudentId == s.Id))
        //            .Select(s => new SelectListItem
        //            {
        //                Value = s.Id.ToString(),
        //                Text = s.FirstName + " " + s.LastName
        //            }).ToList()
        //    };

        //    return View(viewModel);
        //}

        // =======================
        // POST: PRIKAZ NA ZAPISANI STUDENTI
        // =======================

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ManageEnrollments(CourseEnrollmentViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        foreach (var e in model.Enrollments)
        //        {
        //            var enrollment = await _context.Enrollments.FindAsync(e.EnrollmentId);
        //            if (enrollment != null)
        //            {
        //                enrollment.Grade = e.Grade;
        //                enrollment.ExamPoints = e.ExamPoints;
        //                enrollment.SeminalPoints = e.SeminalPoints;
        //                enrollment.ProjectPoints = e.ProjectPoints;
        //                enrollment.AdditionalPoints = e.AdditionalPoints;
        //                enrollment.SeminalUrl = e.SeminalUrl;
        //                enrollment.ProjectUrl = e.ProjectUrl;
        //                enrollment.FinishDate = e.FinishDate;
        //            }
        //        }

        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Edit), new { id = model.CourseId });
        //    }

        //    return View(model);
        //}


        // =======================
        // HELPERS
        // =======================
        private void LoadTeachersDropDowns(int? firstTeacherId = null, int? secondTeacherId = null)
        {
            var teachers = _context.Teachers
                .Select(t => new { t.Id, FullName = t.FirstName + " " + t.LastName })
                .ToList();

            ViewData["FirstTeacherId"] = new SelectList(teachers, "Id", "FullName", firstTeacherId);
            ViewData["SecondTeacherId"] = new SelectList(teachers, "Id", "FullName", secondTeacherId);
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}
