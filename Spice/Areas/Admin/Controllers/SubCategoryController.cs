using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models.ViewModels;

namespace Spice.Areas.Admin.Controllers
{
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubCategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET INDEX
        public async  Task<IActionResult> Index()
        {
            return View(await _context.SubCategories.Include(sc => sc.Category).ToListAsync());
        }

        // GET CREATE
        public async Task<IActionResult> Create()
        {
            var model = new CategoryAndSubCategoryViewModel() {
                Categories = await _context.Categories.ToListAsync(),
                SubCategories = await _context.SubCategories.OrderBy(sc => sc.Name).Select(sc => sc.Name).Distinct().ToListAsync(),
                SubCategory = new Models.SubCategory()
            };

            return View(model);
        }
    }
}
