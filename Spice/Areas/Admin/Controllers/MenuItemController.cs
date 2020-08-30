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

        [BindProperty]
        public IEnumerable<MenuItem> MenuItems { get; set; }

        public MenuItemController(ApplicationDbContext context, IWebHostEnvironment enviroment)
        {
            _context = context;
            _hostingEnviroment = enviroment;
            MenuItemViewModel = new MenuItemViewModel()
            {
                Categories = _context.Categories,
                MenuItem = new MenuItem()
            };
            MenuItems = _context.MenuItems;
        }

        // GET INDEX
        public async Task<IActionResult> Index()
        {
            this.MenuItems = await _context.MenuItems.Include(mi => mi.SubCategory).Include(mi => mi.Category).ToListAsync();
            return View(MenuItems);
        }

        // GET CREATE
        public IActionResult Create()
        {
            return View(MenuItemViewModel);
        }

        // POST CREATE
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
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
                System.IO.File.Copy(fileExtention, webRootPath + @"\images\" + menuItemFromDb.Id + ".png", true);
                menuItemFromDb.Image = @"\images\" + menuItemFromDb.Id + ".png";
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var menuItem = await _context.MenuItems.Include(mi => mi.Category).Include(mi => mi.SubCategory).SingleOrDefaultAsync(mi => mi.Id == id);

            if (menuItem == null)
                return NotFound();

            MenuItemViewModel.MenuItem = menuItem;
            MenuItemViewModel.SubCategories = await _context.SubCategories.Where(sc => sc.CategoryId == menuItem.CategoryId).ToListAsync();

            return View(MenuItemViewModel);
        }


        // POST EDIT
        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("Edit")]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
                return NotFound();


            MenuItemViewModel.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
            {
                MenuItemViewModel.SubCategories = await _context.SubCategories.Where(sc => sc.CategoryId == MenuItemViewModel.MenuItem.CategoryId).ToListAsync();
                return View(MenuItemViewModel);
            }

            // Image saving section
            var menuItemFromDb = await _context.MenuItems.FindAsync(MenuItemViewModel.MenuItem.Id);

            string webRootPath = _hostingEnviroment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            if (files.Count > 0)
            {
                var imagePath = Path.Combine(webRootPath, "images");
                var imageExtention = Path.GetExtension(files[0].FileName);

                // Delete the original File;
                var currentImagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));
                if (System.IO.File.Exists(currentImagePath))
                {
                    System.IO.File.Delete(currentImagePath);
                }

                // upload the new file
                using var fileStream = new FileStream(Path.Combine(imagePath, menuItemFromDb.Id + imageExtention), FileMode.Create);
                files[0].CopyTo(fileStream);
                menuItemFromDb.Image = @"\images\" + menuItemFromDb.Id + imageExtention;
            }

            menuItemFromDb.Name = MenuItemViewModel.MenuItem.Name;
            menuItemFromDb.Description = MenuItemViewModel.MenuItem.Description;
            menuItemFromDb.Price = MenuItemViewModel.MenuItem.Price;
            menuItemFromDb.Spicyness = MenuItemViewModel.MenuItem.Spicyness;
            menuItemFromDb.CategoryId = MenuItemViewModel.MenuItem.CategoryId;
            menuItemFromDb.SubCategoryId = MenuItemViewModel.MenuItem.SubCategoryId;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
