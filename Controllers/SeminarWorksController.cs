using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhaseOne.Data;
using PhaseOne.Models;

namespace PhaseOne.Controllers
{
    public class SeminarWorksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SeminarWorksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SeminarWorks
        public async Task<IActionResult> Index()
        {
            var seminarWorks = await _context.SeminarWork
                .Include(s => s.Student)
                .Include(s => s.Course)
                .ToListAsync();

            return View(seminarWorks);
        }

        // GET: SeminarWorks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var seminarWork = await _context.SeminarWork
                .Include(s => s.Student)
                .Include(s => s.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (seminarWork == null) return NotFound();

            return View(seminarWork);
        }

        // GET: SeminarWorks/Create
        public IActionResult Create()
        {
            ViewData["StudentId"] = new SelectList(
                _context.Students.Select(s => new {
                    s.Id,
                    Display = s.FirstName + " " + s.LastName + " (" + s.StudentId + ")"
                }),
                "Id", "Display"
            );

            ViewData["CourseId"] = new SelectList(
                _context.Courses.Select(c => new {
                    c.Id,
                    Display = c.Title
                }),
                "Id", "Display"
            );

            return View();
        }

        // POST: SeminarWorks/Create (со upload)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SeminarWork seminarWork, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                Directory.CreateDirectory(uploads);

                var filePath = Path.Combine(uploads, file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                seminarWork.FilePath = "/uploads/" + file.FileName;
                seminarWork.UploadDate = DateTime.Now;

                _context.SeminarWork.Add(seminarWork);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Please select a valid file (.doc, .docx, .pdf).");

            // refill dropdowns if error
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "FirstName", seminarWork.StudentId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title", seminarWork.CourseId);

            return View(seminarWork);
        }

        // GET: SeminarWorks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var seminarWork = await _context.SeminarWork.FindAsync(id);
            if (seminarWork == null) return NotFound();

            return View(seminarWork);
        }

        // POST: SeminarWorks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SeminarWork seminarWork)
        {
            if (id != seminarWork.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(seminarWork);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SeminarWorkExists(seminarWork.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(seminarWork);
        }

        // GET: SeminarWorks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var seminarWork = await _context.SeminarWork
                .Include(s => s.Student)
                .Include(s => s.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (seminarWork == null) return NotFound();

            return View(seminarWork);
        }

        // POST: SeminarWorks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var seminarWork = await _context.SeminarWork.FindAsync(id);
            if (seminarWork != null)
            {
                _context.SeminarWork.Remove(seminarWork);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SeminarWorkExists(int id)
        {
            return _context.SeminarWork.Any(e => e.Id == id);
        }
    }
}
