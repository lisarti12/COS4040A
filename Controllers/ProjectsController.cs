using COS4040A.Data;
using COS4040A.Models;
using COS4040A.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace COS4040A.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to add a project.";
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProjectViewModel model)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to add a project.";
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var project = new Project
                {
                    Title = model.Title,
                    Description = model.Description,
                    Author = model.Author,
                    ProgrammingLanguage = model.ProgrammingLanguage,
                    Status = model.Status!.Value,
                    CreatedAt = DateTime.UtcNow,
                    CompletedAt = model.Status == ProjectStatus.Complete ? DateTime.UtcNow : null
                };

                _context.Projects.Add(project);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Project added successfully.";
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred while saving the project.");
                return View(model);
            }
        }
    }
}