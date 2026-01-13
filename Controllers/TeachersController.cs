using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhaseOne.Data;
using PhaseOne.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.IO;


namespace PhaseOne.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TeachersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public TeachersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // =======================
        // GET: Teachers (LIST + FILTER)
        // =======================
        public async Task<IActionResult> Index(string firstName, string lastName, string degree, string academicRank)
        {
            var teachers = _context.Teachers.AsQueryable();

            if (!string.IsNullOrEmpty(firstName))
                teachers = teachers.Where(t => t.FirstName.Contains(firstName));

            if (!string.IsNullOrEmpty(lastName))
                teachers = teachers.Where(t => t.LastName.Contains(lastName));

            if (!string.IsNullOrEmpty(degree))
                teachers = teachers.Where(t => t.Degree.Contains(degree));

            if (!string.IsNullOrEmpty(academicRank))
                teachers = teachers.Where(t => t.AcademicRank.Contains(academicRank));

            // Dropdowns за филтрирање во view (по потреба)
            ViewData["Degree"] = new SelectList(_context.Teachers
                .Select(t => t.Degree)
                .Distinct()
                .OrderBy(d => d));
            ViewData["AcademicRank"] = new SelectList(_context.Teachers
                .Select(t => t.AcademicRank)
                .Distinct()
                .OrderBy(a => a));

            return View(await teachers.ToListAsync());
        }

        // =======================
        // GET: Teachers/Details/5
        // =======================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var teacher = await _context.Teachers
                .Include(t => t.FirstTeacherCourses)
                .Include(t => t.SecondTeacherCourses)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (teacher == null) return NotFound();

            return View(teacher);
        }

        // =======================
        // GET: Teachers/Create
        // =======================
        public IActionResult Create()
        {
            return View();
        }

        // =======================
        // POST: Teachers/Create
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Teacher teacher)
        {
            if (!ModelState.IsValid)
                return View(teacher);

            // 1) Дупликат email во Teachers табела
            if (await _context.Teachers.AnyAsync(t => t.Email == teacher.Email))
            {
                ModelState.AddModelError(nameof(Teacher.Email), "Веќе постои професор со овој е-маил.");
                return View(teacher);
            }

            // 2) Дупликат email во Identity
            if (await _userManager.FindByEmailAsync(teacher.Email) != null)
            {
                ModelState.AddModelError(nameof(Teacher.Email), "Веќе постои корисник со овој е-маил во системот.");
                return View(teacher);
            }

            // 3) Password мора да е внесен (не се чува во Teachers табела)
            if (string.IsNullOrWhiteSpace(teacher.Password))
            {
                ModelState.AddModelError(nameof(Teacher.Password), "Мора да внесете лозинка за професорот.");
                return View(teacher);
            }

            // 4) Креирај Identity user -> лозинката автоматски се хашира
            var user = new ApplicationUser
            {
                UserName = teacher.Email,
                Email = teacher.Email,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user, teacher.Password);
            if (!createResult.Succeeded)
            {
                foreach (var err in createResult.Errors)
                    ModelState.AddModelError("", err.Description);

                return View(teacher);
            }

            // 5) Додели улога Professor
            var roleResult = await _userManager.AddToRoleAsync(user, "Professor");
            if (!roleResult.Succeeded)
            {
                foreach (var err in roleResult.Errors)
                    ModelState.AddModelError("", err.Description);

                // избриши user ако не успее role за да не остане “висечки”
                await _userManager.DeleteAsync(user);
                return View(teacher);
            }

            // 6) Дури сега зачувај Teacher во твојата табела
            _context.Add(teacher);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =======================
        // GET: Teachers/Edit/5
        // =======================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return NotFound();

            return View(teacher);
        }

        // =======================
        // POST: Teachers/Edit/5
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Teacher teacher)
        {
            if (id != teacher.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(teacher.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(teacher);
        }

        // =======================
        // GET: Teachers/Delete/5
        // =======================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(m => m.Id == id);

            if (teacher == null) return NotFound();

            return View(teacher);
        }

        // =======================
        // POST: Teachers/Delete/5
        // =======================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher != null)
                _context.Teachers.Remove(teacher);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //ZA SLIKIIII

        [HttpPost]
        public async Task<IActionResult> UploadProfileImage(int id, IFormFile imageFile)
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

                var teacher = await _context.Teachers.FindAsync(id);
                if (teacher == null) return NotFound();

                teacher.ProfileImagePath = "/images/" + imageFile.FileName;

                _context.Update(teacher);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id });
        }


        // =======================
        // HELPERS
        // =======================
        private bool TeacherExists(int id)
        {
            return _context.Teachers.Any(e => e.Id == id);
        }
    }
}
