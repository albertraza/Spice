using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        [TempData]
        public string StatusMessage { get; set; }

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
                SubCategory = new SubCategory()
            };

            return View(model);
        }

        // POST CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryAndSubCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var category = _context.SubCategories.Include(sc => sc.Category).Where(sc => sc.Name == model.SubCategory.Name && sc.Category.Id == model.SubCategory.CategoryId);

                if (category.Count() > 0)
                {
                    StatusMessage = $"Error: There's a register called {model.SubCategory.Name} in the Category selected, please select a diferent one.";
                }
                else
                {
                    _context.SubCategories.Add(model.SubCategory);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

            }

            return View(new CategoryAndSubCategoryViewModel()
            {
                Categories = await _context.Categories.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategories = await _context.SubCategories.OrderBy(sc => sc.Name).Select(sc => sc.Name).Distinct().ToListAsync(),
                StatusMessage = StatusMessage
            });
        }

        // GET SUBCATEGORIES
        public async Task<IActionResult> GetSubCategories(int id)
        {
            var subCategories = await _context.SubCategories.Where(sc => sc.CategoryId == id).ToListAsync();
            return Json(new SelectList(subCategories, "Id", "Name"));
        }


        // GET EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var subCategory = await _context.SubCategories.FindAsync(id);

            if (subCategory == null)
                return NotFound();

            return View(new CategoryAndSubCategoryViewModel()
            {
                Categories = await _context.Categories.ToListAsync(),
                SubCategory = subCategory,
                SubCategories = await _context.SubCategories.OrderBy(sc => sc.Name).Select(sc => sc.Name).Distinct().ToListAsync()
            });
        }

        // POST EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryAndSubCategoryViewModel model)
        {
            if (id != model.SubCategory.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var categories = _context.SubCategories.Include(sc => sc.Category).Where(sc => sc.Name == model.SubCategory.Name && sc.Category.Id == model.SubCategory.CategoryId);

                if (categories.Count() > 0)
                {
                    StatusMessage = $"Error: There's a register called {model.SubCategory.Name} in the Category selected, please select a diferent one.";
                }
                else
                {
                    var subCategory = await _context.SubCategories.FindAsync(id);
                    subCategory.Name = model.SubCategory.Name;
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(new CategoryAndSubCategoryViewModel()
            {
                Categories = await _context.Categories.ToListAsync(),
                SubCategories = await _context.SubCategories.OrderBy(sc => sc.Name).Select(sc => sc.Name).Distinct().ToListAsync(),
                SubCategory = model.SubCategory,
                StatusMessage = StatusMessage
            });
        }

        // GET DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var subCategory = await _context.SubCategories.FindAsync(id);
            if (subCategory == null)
                return NotFound();

            return View(subCategory);
        }

        // GET DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var subCategory = await _context.SubCategories.FindAsync(id);
            if (subCategory == null)
                return NotFound();
            var category = await _context.Categories.FindAsync(subCategory.CategoryId);
            subCategory.Category = category;

            return View(subCategory);

        }

        //  POST DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var subCategory = await _context.SubCategories.FindAsync(id);

            if (subCategory == null)
                return NotFound();

            _context.SubCategories.Remove(subCategory);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
