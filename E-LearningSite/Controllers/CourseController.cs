using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using E_LearningSite.Data;
using E_LearningSite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;


namespace E_LearningSite.Controllers
{
    [Authorize]
    public class CourseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public CourseController(ApplicationDbContext context, 
            IHostingEnvironment hostingEnvironment, 
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Browse()
        {
            var courses = _context.Courses.ToListAsync();

            return View(await courses);
        }

        // GET: Course
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applicationDbContext = _context.Courses.Where(u => u.TeacherId == userId);

            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Enroll(Guid id)
        {
            var course = await _context.Courses.Include(t => t.Teacher).Include(c => c.CourseStudents)
                .ThenInclude(s => s.Student).SingleOrDefaultAsync(c => c.CourseId == id);

            var userId = _userManager.GetUserId(User);

            if (course.TeacherId == userId || course.CourseStudents.Any(s => s.StudentId == userId))
            {
                return View("Playlist", course.WistiaId);
            }
            else
            {
                return View(course);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Payment(Guid? courseId = null)
        {
            var course = _context.Courses.FirstOrDefault(c => c.CourseId == courseId);
            course.Revenue += course.Price;

            var userId = _userManager.GetUserId(User);
            var model = new CourseStudent
            {
                StudentId = userId,
                CourseId = course.CourseId
            };

            _context.CourseStudents.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Enroll", new {id = courseId});
        }

        // GET: Course/Details/5
            public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Teacher)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Course/Create
        public IActionResult Create()
        {
            ViewBag.TeacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "Id");

            return View();
        }

        // POST: Course/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CourseId,Title,Description,WistiaId,Price,ImagePath,Revenue,TeacherId")] Course course, IFormFile formFile)
        {
            if (ModelState.IsValid)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(formFile.FileName);
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath + "\\files\\", fileName);

                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        course.ImagePath = "/files/" + fileName;
                        await formFile.CopyToAsync(stream);
                    }
                }

                course.CourseId = Guid.NewGuid();

                _context.Add(course);

                if (User.IsInRole(Helpers.EnumRole.Instructor) == false)
                {
                    await _userManager.AddToRoleAsync(await _userManager.GetUserAsync(User), Helpers.EnumRole.Instructor);
                    Helpers.EnumRole.IsInstructor = true;
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            //ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "Id", course.TeacherId);

            return View(course);
        }

        // GET: Course/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.SingleOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }
            //ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "Id", course.TeacherId);

            return View(course);
        }

        // POST: Course/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourseId,Title,Description,WistiaId,Price,ImagePath,Revenue,TeacherId")] Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

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
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            //ViewData["TeacherId"] = new SelectList(_context.Users, "Id", "Id", course.TeacherId);

            return View(course);
        }

        // GET: Course/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Teacher)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.SingleOrDefaultAsync(m => m.Id == id);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}
