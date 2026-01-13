using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhaseOne.Data;
using PhaseOne.Models;
using Microsoft.AspNetCore.Authorization;


namespace PhaseOne.Controllers
{
    [Authorize(Roles = "Admin")]
    
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



        [Authorize(Roles = "Admin")]

        public IActionResult AdminEditEnrollments(int courseId)
        {
            var course = _context.Courses
                .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student)
                .FirstOrDefault(c => c.Id == courseId);

            if (course == null) return NotFound();

            var allStudents = _context.Students.ToList();

            var vm = new CourseEnrollmentViewModel
            {
                CourseId = courseId,
                Year = DateTime.Now.Year,
                Semester = "Winter",
                Students = allStudents.Select(s => new SelectableStudent
                {
                    StudentId = s.Id,
                    FullName = s.StudentId + " - " + s.FirstName + " " + s.LastName,
                    IsEnrolled = false // ќе го решиме во следен чекор
                }).ToList()
            };

            ViewBag.CourseTitle = course.Title;
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult AdminUpdateEnrollments(int courseId, int year, string semester, List<long> selectedStudentIds)
        {
            // постоечки enrollments за ова course + year + semester
            var existing = _context.Enrollments
                .Where(e => e.CourseId == courseId && e.Year == year && e.Semester == semester)
                .ToList();

            foreach (var studentId in selectedStudentIds)
            {
                bool already = existing.Any(e => e.StudentId == studentId);
                if (!already)
                {
                    _context.Enrollments.Add(new Enrollment
                    {
                        CourseId = courseId,
                        StudentId = studentId,
                        Year = year,
                        Semester = semester
                        // останато null
                    });
                }
            }

            _context.SaveChanges();
            return RedirectToAction("Details", new { id = courseId });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminDeactivateEnrollments(int courseId, int year, string semester)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return NotFound();

            var active = _context.Enrollments
                .Where(e => e.CourseId == courseId
                            && e.Year == year
                            && e.Semester == semester
                            && e.FinishDate == null)
                .Select(e => new EnrollmentDeactivateItem
                {
                    EnrollmentId = e.Id,
                    StudentId = e.StudentId,
                    StudentDisplay = e.Student.StudentId + " - " + e.Student.FirstName + " " + e.Student.LastName
                })
                .ToList();

            var vm = new AdminDeactivateEnrollmentsViewModel
            {
                CourseId = courseId,
                Year = year,
                Semester = semester,
                FinishDate = DateTime.Today,
                Enrollments = active
            };

            ViewBag.CourseTitle = course.Title;
            return View(vm);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult AdminDeactivateEnrollments(AdminDeactivateEnrollmentsViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // ако има грешка, мора повторно да ја наполниме листата
                vm.Enrollments = _context.Enrollments
                    .Where(e => e.CourseId == vm.CourseId
                                && e.Year == vm.Year
                                && e.Semester == vm.Semester
                                && e.FinishDate == null)
                    .Select(e => new EnrollmentDeactivateItem
                    {
                        EnrollmentId = e.Id,
                        StudentId = e.StudentId,
                        StudentDisplay = e.Student.StudentId + " - " + e.Student.FirstName + " " + e.Student.LastName
                    })
                    .ToList();

                return View(vm);
            }

            var rows = _context.Enrollments
                .Where(e => vm.SelectedEnrollmentIds.Contains(e.Id) && e.FinishDate == null)
                .ToList();

            foreach (var e in rows)
                e.FinishDate = vm.FinishDate;

            _context.SaveChanges();
            return RedirectToAction("Details", new { id = vm.CourseId });
        }

    }
}
