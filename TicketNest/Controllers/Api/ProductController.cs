using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketNest.Data;
using TicketNest.Models;

namespace TicketNest.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/Product")]
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{organizationId}")]
        public IActionResult GetProduct([FromRoute] Guid organizationId)
        {
            return Json(new { data = _context.Product.Where(x => x.organizationId.Equals(organizationId)).ToList() });
        }

        [HttpPost]
        public async Task<IActionResult> PostProduct([FromBody] Product product)
        {
            try
            {
                ModelState.Remove("CreateBy");
                ModelState.Remove("organization");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (product.productId == Guid.Empty)
                {
                    product.productId = Guid.NewGuid();
                    product.CreateBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    _context.Product.Add(product);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Add new data success." });
                }
                else
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Edit data success." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid id)
        {
            try
            {
                var product = await _context.Product.SingleOrDefaultAsync(m => m.productId == id);
                if (product == null) return NotFound();

                _context.Product.Remove(product);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Delete success." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private bool ProductExists(Guid id)
        {
            return _context.Product.Any(e => e.productId == id);
        }
    }
}