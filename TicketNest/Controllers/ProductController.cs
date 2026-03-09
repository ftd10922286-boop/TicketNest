using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TicketNest.Data;
using TicketNest.Models;

namespace TicketNest.Controllers
{
    public class ProductController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(Guid org)
        {
            if (org == Guid.Empty)
            {
                var appUser = await _userManager.GetUserAsync(User);
                if (appUser == null) return RedirectToAction("Login", "Account");

                var userOrg = _context.Organization
                    .Where(x => x.organizationOwnerId.Equals(appUser.Id))
                    .FirstOrDefault();

                if (userOrg != null) org = userOrg.organizationId;
            }

            if (org == Guid.Empty) return RedirectToAction("Index", "Config");

            Organization organization = _context.Organization
                .Where(x => x.organizationId.Equals(org))
                .FirstOrDefault();

            ViewData["org"] = org;
            return View(organization);
        }

        public IActionResult AddEdit(Guid org, Guid id)
        {
            if (id == Guid.Empty)
            {
                Product product = new Product();
                product.organizationId = org;
                return View(product);
            }
            else
            {
                return View(_context.Product.Where(x => x.productId.Equals(id)).FirstOrDefault());
            }
        }
    }
}