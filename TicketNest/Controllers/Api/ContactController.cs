using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;
using TicketNest.Data;
using TicketNest.Models;
using TicketNest.Services;

namespace TicketNest.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/Contact")]
    [Authorize]
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDotnetdesk _dotnetdesk;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ContactController(ApplicationDbContext context,
            IDotnetdesk dotnetdesk,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender)
        {
            _context = context;
            _dotnetdesk = dotnetdesk;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpGet("{customerId}")]
        public IActionResult GetContact([FromRoute] Guid customerId)
        {
            return Json(new { data = _context.Contact.Where(x => x.customerId.Equals(customerId)).ToList() });
        }

        [HttpPost]
        public async Task<IActionResult> PostContact([FromBody] Contact contact)
        {
            try
            {
                ModelState.Remove("CreateBy");
                ModelState.Remove("applicationUser");
                ModelState.Remove("customer");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (contact.contactId == Guid.Empty)
                {
                    var user = new ApplicationUser
                    {
                        UserName = contact.email,
                        Email = contact.email,
                        FullName = contact.contactName
                    };
                    user.IsCustomer = true;

                    var randomPassword = new Random().Next(0, 999999);
                    var result = await _userManager.CreateAsync(user, randomPassword.ToString());

                    if (result.Succeeded)
                    {
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);

                        await _emailSender.SendEmailAsync(contact.email, "Confirm your email and Registration",
                            $"Your email has been registered. With username:'{contact.email}' and temporary password:'{randomPassword}'. Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>link</a>");

                        contact.applicationUser = user;
                        contact.contactId = Guid.NewGuid();
                        contact.CreateBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                        _context.Contact.Add(contact);
                        await _context.SaveChangesAsync();
                        return Json(new { success = true, message = "Add new data success." });
                    }
                    else
                    {
                        return Json(new { success = false, message = "UserManager CreateAsync Fail." });
                    }
                }
                else
                {
                    _context.Update(contact);
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
        public async Task<IActionResult> DeleteContact([FromRoute] Guid id)
        {
            try
            {
                var contact = await _context.Contact.SingleOrDefaultAsync(m => m.contactId == id);
                if (contact == null) return NotFound();

                _context.Contact.Remove(contact);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Delete success." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private bool ContactExists(Guid id)
        {
            return _context.Contact.Any(e => e.contactId == id);
        }
    }
}