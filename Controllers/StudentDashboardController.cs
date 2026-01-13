using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhaseOne.Data;
using PhaseOne.ViewModels;

namespace PhaseOne.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ 1) Листа на мои предмети (како што бараш - со статус/поени/оцена)
        public async Task<IActionResult> Index()
        {
            var email = User?.Identity?.Name;
            if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

            var student = await _context.Students.AsNoTracking()
                .FirstOrDefaultAsync(s => s.Email == email);

            if (student == null) return Forbid();

            var myEnrollments = await _context.Enrollments
                .AsNoTracking()
                .Where(e => e.StudentId == student.Id)
                .Include(e => e.Course)
                .OrderByDescending(e => e.Year)
                .ThenBy(e => e.Course.Title)
                .Select(e => new
                {
                    EnrollmentId = e.Id,
                    CourseTitle = e.Course.Title,
                    e.Year,
                    Semester = e.Semester ?? "",

                    e.ExamPoints,
                    e.SeminalPoints,
                    e.ProjectPoints,
                    e.AdditionalPoints,

                    e.Grade,
                    e.FinishDate,
                    IsActive = e.FinishDate == null,

                    // ✅ ново
                    SeminalUrl = e.SeminalUrl,
                    ProjectUrl = e.ProjectUrl
                })
                .ToListAsync();

            return View(myEnrollments);
        }

        // ✅ 2) Уредување линкови за конкретен enrollment (GET)
        [HttpGet]
        public async Task<IActionResult> EditLinks(long enrollmentId)
        {
            var email = User?.Identity?.Name;
            if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

            var student = await _context.Students.AsNoTracking()
                .FirstOrDefaultAsync(s => s.Email == email);

            if (student == null) return Forbid();

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == enrollmentId);

            if (enrollment == null) return NotFound();

            // ✅ студентот смее да уредува само свои запишувања
            if (enrollment.StudentId != student.Id) return Forbid();

            // (ако сакаш да НЕ дозволиш уредување кога е завршен, одкоментирај)
            // if (enrollment.FinishDate != null) return Forbid();

            var vm = new StudentEditLinksVm
            {
                EnrollmentId = enrollment.Id,
                CourseTitle = enrollment.Course?.Title ?? "",
                Year = enrollment.Year,

                CurrentSeminalUrl = enrollment.SeminalUrl,
                ProjectUrl = enrollment.ProjectUrl
            };

            return View(vm);
        }

        // ✅ 3) Уредување линкови (POST) - upload + github link
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLinks(StudentEditLinksVm vm)
        {
            var email = User?.Identity?.Name;
            if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

            var student = await _context.Students.AsNoTracking()
                .FirstOrDefaultAsync(s => s.Email == email);

            if (student == null) return Forbid();

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == vm.EnrollmentId);

            if (enrollment == null) return NotFound();

            if (enrollment.StudentId != student.Id) return Forbid();

            // (ако сакаш да НЕ дозволиш уредување кога е завршен, одкоментирај)
            // if (enrollment.FinishDate != null)
            // {
            //     ModelState.AddModelError("", "Овој предмет е завршен и не може да се уредуваат линковите.");
            //     vm.CourseTitle = enrollment.Course?.Title ?? "";
            //     vm.CurrentSeminalUrl = enrollment.SeminalUrl;
            //     return View(vm);
            // }

            // ✅ 1) Upload на семинарска (doc/docx/pdf)
            if (vm.SeminarFile != null && vm.SeminarFile.Length > 0)
            {
                var ext = Path.GetExtension(vm.SeminarFile.FileName).ToLowerInvariant();
                var allowed = new[] { ".pdf", ".doc", ".docx" };

                if (!allowed.Contains(ext))
                {
                    ModelState.AddModelError("", "Дозволени формати: .doc, .docx, .pdf");
                    vm.CourseTitle = enrollment.Course?.Title ?? "";
                    vm.CurrentSeminalUrl = enrollment.SeminalUrl;
                    return View(vm);
                }

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "seminars");
                Directory.CreateDirectory(uploadsFolder);

                var safeName = $"{enrollment.Id}_{DateTime.UtcNow:yyyyMMddHHmmss}{ext}";
                var fullPath = Path.Combine(uploadsFolder, safeName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await vm.SeminarFile.CopyToAsync(stream);
                }

                // ✅ се зачувува во база како URL до wwwroot
                enrollment.SeminalUrl = "/uploads/seminars/" + safeName;
            }

            // ✅ 2) ProjectUrl (GitHub repo link)
            if (!string.IsNullOrWhiteSpace(vm.ProjectUrl))
            {
                var url = vm.ProjectUrl.Trim();

                if (!url.StartsWith("https://github.com/", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("", "ProjectURL мора да биде GitHub линк (https://github.com/...).");
                    vm.CourseTitle = enrollment.Course?.Title ?? "";
                    vm.CurrentSeminalUrl = enrollment.SeminalUrl;
                    return View(vm);
                }

                enrollment.ProjectUrl = url;
            }
            else
            {
                enrollment.ProjectUrl = null;
            }

            await _context.SaveChangesAsync();

            TempData["Ok"] = "Линковите се успешно зачувани.";
            return RedirectToAction(nameof(Index));
        }
    }
}
