using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketNest.Data;
using TicketNest.Models;

namespace src.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/Customer")]
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{organizationId}")]
        public IActionResult GetCustomer([FromRoute] Guid organizationId)
        {
            return Json(new { data = _context.Customer.Where(x => x.organizationId.Equals(organizationId)).ToList() });
        }

        [HttpPost]
        public async Task<IActionResult> PostCustomer([FromBody] Customer customer)
        {
            try
            {
                ModelState.Remove("CreateBy");
                ModelState.Remove("organization");
                ModelState.Remove("contacts");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (customer.customerId == Guid.Empty)
                {
                    customer.customerId = Guid.NewGuid();
                    customer.CreateBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    _context.Customer.Add(customer);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Add new data success." });
                }
                else
                {
                    _context.Update(customer);
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
        public async Task<IActionResult> DeleteCustomer([FromRoute] Guid id)
        {
            try
            {
                var customer = await _context.Customer.SingleOrDefaultAsync(m => m.customerId == id);
                if (customer == null) return NotFound();

                _context.Customer.Remove(customer);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Delete success." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private bool CustomerExists(Guid id)
        {
            return _context.Customer.Any(e => e.customerId == id);
        }
    }
}