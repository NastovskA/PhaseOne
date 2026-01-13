using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhaseOne.Data;
using PhaseOne.Models;
using System.Threading.Tasks;

namespace PhaseOne.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminDashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // --- Dashboard статистика ---
        public async Task<IActionResult> Index()
        {
            ViewBag.CourseCount = await _context.Courses.CountAsync();
            ViewBag.StudentCount = await _context.Students.CountAsync();
            ViewBag.TeacherCount = await _context.Teachers.CountAsync();
            ViewBag.EnrollmentCount = await _context.Enrollments.CountAsync();
            return View();
        }

        // --- Додавање студент ---
        //[HttpGet]
        //public IActionResult CreateStudent()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public async Task<IActionResult> CreateStudent(Student student)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Students.Add(student);
        //        await _context.SaveChangesAsync();

        //        var user = new ApplicationUser
        //        {
        //            UserName = student.Email,
        //            Email = student.Email,
        //            EmailConfirmed = true
        //        };

        //        var result = await _userManager.CreateAsync(user, student.Password);
        //        if (result.Succeeded)
        //        {
        //            await _userManager.AddToRoleAsync(user, "Student");
        //            return RedirectToAction(nameof(Index));
        //        }

        //        foreach (var error in result.Errors)
        //        {
        //            ModelState.AddModelError("", error.Description);
        //        }
        //    }
        //    return View(student);
        //}

        //// --- Додавање професор ---
        //[HttpGet]
        //public IActionResult CreateProfessor()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> CreateProfessor(Teacher teacher)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Teachers.Add(teacher);
        //        await _context.SaveChangesAsync();

        //        var user = new ApplicationUser
        //        {
        //            UserName = teacher.Email,
        //            Email = teacher.Email,
        //            EmailConfirmed = true
        //        };

        //        var result = await _userManager.CreateAsync(user, teacher.Password);
        //        if (result.Succeeded)
        //        {
        //            await _userManager.AddToRoleAsync(user, "Professor");
        //            return RedirectToAction(nameof(Index));
        //        }

        //        foreach (var error in result.Errors)
        //        {
        //            ModelState.AddModelError("", error.Description);
        //        }
        //    }
        //    return View(teacher);
        //}
    }
}
