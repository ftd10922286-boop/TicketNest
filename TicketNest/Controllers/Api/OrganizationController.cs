using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketNest.Data;
using TicketNest.Models;

namespace TicketNest.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/Organization")]
    [Authorize]
    public class OrganizationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrganizationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{applicationUserId}")]
        public IActionResult GetOrganization([FromRoute] string applicationUserId)
        {
            return Json(new
            {
                data = _context.Organization
                .Where(x => x.organizationOwnerId.Equals(applicationUserId))
                .OrderByDescending(x => x.CreateAt)
                .ToList()
            });
        }

        [HttpPost]
        public async Task<IActionResult> PostOrganization([FromBody] Organization organization)
        {
            try
            {
                organization.organizationOwnerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                ModelState.Remove("organizationOwnerId");
                ModelState.Remove("CreateBy");
                ModelState.Remove("tickets");
                ModelState.Remove("products");
                ModelState.Remove("customers");
                ModelState.Remove("supportAgents");
                ModelState.Remove("supportEngineers");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (organization.organizationId == Guid.Empty)
                {
                    organization.organizationId = Guid.NewGuid();
                    organization.CreateBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    _context.Organization.Add(organization);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Add new data success." });
                }
                else
                {
                    _context.Update(organization);
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
        public async Task<IActionResult> DeleteOrganization([FromRoute] Guid id)
        {
            try
            {
                var organization = await _context.Organization.SingleOrDefaultAsync(m => m.organizationId == id);
                if (organization == null) return NotFound();

                _context.Organization.Remove(organization);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Delete success." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private bool OrganizationExists(Guid id)
        {
            return _context.Organization.Any(e => e.organizationId == id);
        }
    }
}