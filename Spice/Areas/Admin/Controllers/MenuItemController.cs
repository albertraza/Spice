using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utils;

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

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost() {
            MenuItemViewModel.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
                return View(MenuItemViewModel);

            _context.MenuItems.Add(MenuItemViewModel.MenuItem);
            await _context.SaveChangesAsync();

            //Work on the image saving section

            string webRootPath = _hostingEnviroment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = await _context.MenuItems.FindAsync(MenuItemViewModel.MenuItem.Id);

            if (files.Count > 0)
            {
                var uploadPath = Path.Combine(webRootPath, "images");
                var fileExtention = Path.GetExtension(files[0].FileName);

                using var fileStream = new FileStream(Path.Combine(uploadPath, menuItemFromDb.Id + fileExtention), FileMode.Create);
                files[0].CopyTo(fileStream);

                menuItemFromDb.Image = @"\images\" + menuItemFromDb.Id + fileExtention;

            }
            else
            {
                var fileExtention = Path.Combine(webRootPath, @"images\" + Util.DefaultImage);
                System.IO.File.Copy(fileExtention, webRootPath + @"\images\" + menuItemFromDb.Id + ".png");
                menuItemFromDb.Image = @"\images\" + menuItemFromDb.Id + ".png";
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
