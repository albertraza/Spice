using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Extentions;
using Spice.Models;
using Spice.Models.ViewModels;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubCategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET INDEX
        public async Task<IActionResult> Index()
        {
            return View(await _context.SubCategories.Include(sc => sc.Category).ToListAsync());
        }

        // GET CREATE
        public async Task<IActionResult> Create()
        {
            var model = new CategoryAndSubCategoryViewModel()
            {
                Categories = await _context.Categories.ToListAsync(),
                SubCategories = await _context.SubCategories.OrderBy(sc => sc.Name).Select(sc => sc.Name).Distinct().ToListAsync(),
                SubCategory = new Models.SubCategory()
            };

            return View(model);
        }

        // POST CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryAndSubCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var NewModel = new CategoryAndSubCategoryViewModel()
                {
                    Categories = await _context.Categories.ToListAsync(),
                    SubCategory = model.SubCategory,
                    SubCategories = await _context.SubCategories.OrderBy(sc => sc.Name).Select(sc => sc.Name).Distinct().ToListAsync()
                };
                return View(NewModel);
            }

            var category = _context.SubCategories.Include(sc => sc.Category).Where(sc => sc.Name == model.SubCategory.Name && sc.Category.Id == model.SubCategory.CategoryId);

            if (category.Count() > 0)
            {
                var NewModel = new CategoryAndSubCategoryViewModel()
                {
                    Categories = await _context.Categories.ToListAsync(),
                    SubCategory = model.SubCategory,
                    SubCategories = await _context.SubCategories.OrderBy(sc => sc.Name).Select(sc => sc.Name).Distinct().ToListAsync(),
                    StatusMessage = "There's a sub category registered with the same name in the same category, use a different name"
                };
                return View(NewModel);
            }
            else
            {
                _context.SubCategories.Add(model.SubCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
