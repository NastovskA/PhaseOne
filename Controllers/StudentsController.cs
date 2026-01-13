using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhaseOne.Data;
using PhaseOne.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;


namespace PhaseOne.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
            if (!ModelState.IsValid)
                return View(student);

            // 1) Проверки за дупликат email
            if (await _context.Students.AnyAsync(s => s.Email == student.Email))
            {
                ModelState.AddModelError(nameof(Student.Email), "Веќе постои студент со овој е-маил.");
                return View(student);
            }

            if (await _userManager.FindByEmailAsync(student.Email) != null)
            {
                ModelState.AddModelError(nameof(Student.Email), "Веќе постои корисник со овој е-маил во системот.");
                return View(student);
            }

            // 2) Password мора да е внесен (не се чува во Students табела!)
            if (string.IsNullOrWhiteSpace(student.Password))
            {
                ModelState.AddModelError(nameof(Student.Password), "Мора да внесете лозинка за студентот.");
                return View(student);
            }

            // 3) Креирај Identity user -> лозинката автоматски се хашира во AspNetUsers
            var user = new ApplicationUser
            {
                UserName = student.Email,
                Email = student.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, student.Password);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError("", err.Description);

                return View(student);
            }

            // 4) Додели улога
            await _userManager.AddToRoleAsync(user, "Student");

            // 5) Зачувај Student во твојата табела
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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
