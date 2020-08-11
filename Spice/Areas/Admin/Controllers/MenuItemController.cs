using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnviroment;

        [BindProperty]
        public MenuItemViewModel MenuItemViewModel { get; set; }

        public MenuItemController(ApplicationDbContext context, IWebHostEnvironment enviroment)
        {
            _context = context;
            _hostingEnviroment = enviroment;
            MenuItemViewModel = new MenuItemViewModel()
            {
                Categories = _context.Categories,
                MenuItem = new MenuItem()
            };
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.MenuItems.Include(mi => mi.Category).Include(mi => mi.SubCategory).ToListAsync());
        }

        public IActionResult Create()
        {
            return View(MenuItemViewModel);
        }
    }
}
