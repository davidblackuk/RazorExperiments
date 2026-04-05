using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FirstApp.Data;
using FirstApp.Models;

namespace FirstApp.Pages.Repositories
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Repository Repository { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var repository = await _context.Repositories.FirstOrDefaultAsync(m => m.Id == id);
            
            if (repository == null)
            {
                return NotFound();
            }
            
            Repository = repository;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Forbid();
            }

            // Load the existing entity from the database
            var existingRepository = await _context.Repositories
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == Repository.Id);

            if (existingRepository == null)
            {
                return NotFound();
            }

            // Update only the fields that should change
            Repository.UpdatedById = userId;
            Repository.UpdatedAt = DateTime.UtcNow;

            // Remove validation errors for navigation properties since they're not bound from the form
            ModelState.Remove("Repository.CreatedBy");
            ModelState.Remove("Repository.UpdatedBy");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Repository).State = EntityState.Modified;
            // Don't modify the navigation properties
            _context.Entry(Repository).Reference(r => r.CreatedBy).IsModified = false;
            _context.Entry(Repository).Reference(r => r.UpdatedBy).IsModified = false;

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private bool RepositoryExists(int id)
        {
            return _context.Repositories.Any(e => e.Id == id);
        }
    }
}
