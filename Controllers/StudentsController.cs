using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhaseOne.Data;
using PhaseOne.Models;

namespace PhaseOne.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =======================
        // GET: Students (LIST + FILTER)
        // =======================
        public async Task<IActionResult> Index(string studentId, string firstName, string lastName, int? courseId)
        {
            var students = _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .AsQueryable();

            // Филтрирање по индекс
            if (!string.IsNullOrEmpty(studentId))
                students = students.Where(s => s.StudentId.Contains(studentId));

            // Филтрирање по име
            if (!string.IsNullOrEmpty(firstName))
                students = students.Where(s => s.FirstName.Contains(firstName));

            // Филтрирање по презиме
            if (!string.IsNullOrEmpty(lastName))
                students = students.Where(s => s.LastName.Contains(lastName));

            // Филтрирање по предмет
            if (courseId.HasValue)
                students = students.Where(s => s.Enrollments.Any(e => e.CourseId == courseId));

            // Dropdown за избор на предмет за филтрирање
            ViewData["CourseId"] = new SelectList(_context.Courses
                .OrderBy(c => c.Title)
                .ToList(), "Id", "Title", courseId);

            return View(await students.ToListAsync());
        }

        // =======================
        // GET: Students/Details/5
        // =======================
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (student == null) return NotFound();

            return View(student);
        }

        // =======================
        // GET: Students/Create
        // =======================
        public IActionResult Create()
        {
            return View();
        }

        // =======================
        // POST: Students/Create
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // =======================
        // GET: Students/Edit/5
        // =======================
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            return View(student);
        }

        // =======================
        // POST: Students/Edit/5
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Student student)
        {
            if (id != student.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // =======================
        // GET: Students/Delete/5
        // =======================
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);

            if (student == null) return NotFound();

            return View(student);
        }

        // =======================
        // POST: Students/Delete/5
        // =======================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
                _context.Students.Remove(student);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //ZA SLIKIIII

        [HttpPost]
        public async Task<IActionResult> UploadProfileImage(long id, IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                Directory.CreateDirectory(uploads);

                var filePath = Path.Combine(uploads, imageFile.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                var student = await _context.Students.FindAsync(id);
                if (student == null) return NotFound();

                student.ProfileImagePath = "/images/" + imageFile.FileName;

                _context.Update(student);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id });
        }



        // =======================
        // HELPERS
        // =======================
        private bool StudentExists(long id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
}
