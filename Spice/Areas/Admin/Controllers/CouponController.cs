
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CouponController : Controller
    {
        ApplicationDbContext _context;

        [BindProperty]
        public IEnumerable<Coupon> Coupons { get; set; }

        [BindProperty]
        public Coupon Coupon { get; set; }

        public CouponController(ApplicationDbContext context)
        {
            _context = context;
            Coupons = _context.Coupons;
            Coupon = new Coupon();
        }
        public async Task<IActionResult> Index()
        {
            Coupons = await _context.Coupons.ToListAsync();
            return View(Coupons);
        }

        public IActionResult Create()
        {
            return View(Coupon);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("Create")]
        public async Task<IActionResult> CreatePost()
        {
            if (!ModelState.IsValid)
                return View(Coupon);

            var files = HttpContext.Request.Form.Files;
            if (files.Count > 0)
            {
                byte[] picture = null;
                using var fileStream = files[0].OpenReadStream();
                using var memoryStream = new MemoryStream();
                fileStream.CopyTo(memoryStream);
                picture = memoryStream.ToArray();
                Coupon.Picture = picture;
            }

            _context.Coupons.Add(Coupon);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            Coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Id == id);

            if (Coupon == null)
                return NotFound();

            return View(Coupon);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("Edit")]
        public async Task<IActionResult> EditPost()
        {
            if (!ModelState.IsValid)
                return View(Coupon);

            var files = HttpContext.Request.Form.Files;
            var couponFromDb = await _context.Coupons.FirstOrDefaultAsync(c => c.Id == Coupon.Id);

            if (couponFromDb == null)
                return NotFound();

            if (files.Count > 0)
            {
                byte[] pictureFile = null;
                using var fileStream = files[0].OpenReadStream();
                using var memoryStream = new MemoryStream();
                fileStream.CopyTo(memoryStream);
                pictureFile = memoryStream.ToArray();
                couponFromDb.Picture = pictureFile;
            }

            couponFromDb.Name = Coupon.Name;
            couponFromDb.CouponType = Coupon.CouponType;
            couponFromDb.Discount = Coupon.Discount;
            couponFromDb.MinimumAmout = Coupon.MinimumAmout;
            couponFromDb.IsActive = Coupon.IsActive;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            Coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Id == id);

            if (Coupon == null)
                return NotFound();

            return View(Coupon);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            Coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Id == id);

            if (Coupon == null)
                return NotFound();

            return View(Coupon);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id == null)
                return NotFound();

            Coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Id == id);

            if (Coupon == null)
                return NotFound();

            _context.Coupons.Remove(Coupon);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}