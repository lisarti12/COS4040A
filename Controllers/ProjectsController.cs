using COS4040A.Data;
using COS4040A.Models;
using COS4040A.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

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

        [HttpGet]
        public IActionResult Search()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to search projects.";
                return RedirectToAction("Login", "Account");
            }

            return View(new SearchProjectsViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(SearchProjectsViewModel model)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to search projects.";
                return RedirectToAction("Login", "Account");
            }

            model.Results = new List<SearchResultViewModel>();

            if (string.IsNullOrWhiteSpace(model.SearchTerm))
            {
                ModelState.AddModelError(string.Empty, "Please enter a search term.");
                return View(model);
            }

            string term = model.SearchTerm.Trim();

            var projects = await _context.Projects.ToListAsync();

            var results = projects
                .Select(p => new SearchResultViewModel
                {
                    Project = p,
                    MatchCount =
                        CountOccurrences(p.Title, term) +
                        CountOccurrences(p.Description, term) +
                        CountOccurrences(p.Author, term) +
                        CountOccurrences(p.ProgrammingLanguage, term)
                })
                .Where(x => x.MatchCount > 0)
                .OrderByDescending(x => x.MatchCount)
                .ThenBy(x => x.Project.Title)
                .ToList();

            model.Results = results;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to delete projects.";
                return RedirectToAction("Login", "Account");
            }

            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                TempData["ErrorMessage"] = "Project not found.";
                return RedirectToAction("Index", "Home");
            }

            if (project.Status == ProjectStatus.InProgress)
            {
                TempData["ErrorMessage"] = "In-progress projects cannot be deleted.";
                return RedirectToAction("Index", "Home");
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Project '{project.Title}' was deleted successfully.";
            return RedirectToAction("Index", "Home");
        }

        private static int CountOccurrences(string? source, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(searchTerm))
                return 0;

            int count = 0;
            int index = 0;

            while ((index = source.IndexOf(searchTerm, index, StringComparison.OrdinalIgnoreCase)) != -1)
            {
                count++;
                index += searchTerm.Length;
            }

            return count;
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to edit projects.";
                return RedirectToAction("Login", "Account");
            }

            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                TempData["ErrorMessage"] = "Project not found.";
                return RedirectToAction("Index", "Home");
            }

            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Project model)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to edit projects.";
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var project = await _context.Projects.FindAsync(model.Id);

            if (project == null)
            {
                TempData["ErrorMessage"] = "Project not found.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                project.Title = model.Title;
                project.Description = model.Description;
                project.Author = model.Author;
                project.ProgrammingLanguage = model.ProgrammingLanguage;
                project.Status = model.Status;

                project.CompletedAt = model.Status == ProjectStatus.Complete
                    ? DateTime.UtcNow
                    : null;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Project updated successfully.";
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                ModelState.AddModelError("", "Error updating project.");
                return View(model);
            }

        }

        [HttpGet]
        public IActionResult Import()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to import projects.";
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to import projects.";
                return RedirectToAction("Login", "Account");
            }

            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Please upload a valid JSON file.";
                return RedirectToAction("Import");
            }

            try
            {
                string json;

                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    json = await reader.ReadToEndAsync();
                }

                // Load schema
                var schemaPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "schemas", "project-schema.json");
                var schemaJson = System.IO.File.ReadAllText(schemaPath);
                var schema = JSchema.Parse(schemaJson);

                // Parse JSON
                var jArray = JArray.Parse(json);

                // Validate JSON
                if (!jArray.IsValid(schema, out IList<string> errors))
                {
                    TempData["ErrorMessage"] = "Invalid file: " + string.Join("; ", errors);
                    return RedirectToAction("Import");
                }

                int count = 0;

                foreach (var item in jArray)
                {
                    var project = new Project
                    {
                        Title = item["title"]!.ToString(),
                        Description = item["description"]!.ToString(),
                        Author = item["author"]!.ToString(),
                        ProgrammingLanguage = item["programmingLanguage"]!.ToString(),
                        Status = Enum.Parse<ProjectStatus>(item["status"]!.ToString()),
                        CreatedAt = DateTime.UtcNow,
                        CompletedAt = item["status"]!.ToString() == "Complete"
                            ? DateTime.UtcNow
                            : null
                    };

                    _context.Projects.Add(project);
                    count++;
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"{count} projects imported successfully.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error processing file: " + ex.Message;
                return RedirectToAction("Import");
            }
        }
    }
}