using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketNest.Data;
using TicketNest.Models;

namespace TicketNest.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/Ticket")]
    [Authorize]
    public class TicketController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{organizationId}")]
        public IActionResult GetTicket([FromRoute] Guid organizationId)
        {
            return Json(new { data = _context.Ticket.Where(x => x.organizationId.Equals(organizationId)).ToList() });
        }

        [HttpGet("Customer/{customerId}")]
        public IActionResult GetTicketCustomer([FromRoute] Guid customerId)
        {
            return Json(new { data = _context.Ticket.Where(x => x.customerId.Equals(customerId)).ToList() });
        }

        [HttpPost]
        public async Task<IActionResult> PostTicket([FromBody] Ticket ticket)
        {
            try
            {
                ModelState.Remove("CreateBy");
                ModelState.Remove("organization");
                ModelState.Remove("product");
                ModelState.Remove("supportAgent");
                ModelState.Remove("supportEngineer");
                ModelState.Remove("contact");
                ModelState.Remove("customer");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (ticket.ticketId == Guid.Empty)
                {
                    Contact contact = _context.Contact.Where(x => x.contactId.Equals(ticket.contactId)).FirstOrDefault();
                    ticket.ticketId = Guid.NewGuid();
                    ticket.customerId = contact.customerId;
                    ticket.CreateBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    _context.Ticket.Add(ticket);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Add new data success." });
                }
                else
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Edit data success." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("Customer")]
        public async Task<IActionResult> PostTicketCustomer([FromBody] Ticket ticket)
        {
            try
            {
                ModelState.Remove("CreateBy");
                ModelState.Remove("organization");
                ModelState.Remove("product");
                ModelState.Remove("supportAgent");
                ModelState.Remove("supportEngineer");
                ModelState.Remove("contact");
                ModelState.Remove("customer");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (ticket.ticketId == Guid.Empty)
                {
                    ticket.ticketId = Guid.NewGuid();
                    ticket.CreateBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    _context.Ticket.Add(ticket);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Add new data success." });
                }
                else
                {
                    _context.Update(ticket);
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
        public async Task<IActionResult> DeleteTicket([FromRoute] Guid id)
        {
            try
            {
                var ticket = await _context.Ticket.SingleOrDefaultAsync(m => m.ticketId == id);
                if (ticket == null) return NotFound();

                _context.Ticket.Remove(ticket);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Delete success." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("Customer/{id}")]
        public async Task<IActionResult> DeleteTicketCustomer([FromRoute] Guid id)
        {
            try
            {
                var ticket = await _context.Ticket.SingleOrDefaultAsync(m => m.ticketId == id);
                if (ticket == null) return NotFound();

                _context.Ticket.Remove(ticket);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Delete success." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private bool TicketExists(Guid id)
        {
            return _context.Ticket.Any(e => e.ticketId == id);
        }
    }
}