
using System.Collections.Generic;
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

        CouponController(ApplicationDbContext context)
        {
            _context = context;
            Coupons = _context.Coupons;
        }
        public async Task<IActionResult> Index()
        {
            Coupons = await _context.Coupons.ToListAsync();
            return View(Coupons);
        }
    }
}