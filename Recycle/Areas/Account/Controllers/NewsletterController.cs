using Recycle.App_Custom.ActionFilters;
using Recycle.Areas.Account.ViewModels.Newsletter;
using Recycle.Data;
using Recycle.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Recycle.Areas.Account.Controllers
{
    [Area("Account")]
    public class NewsletterController : Controller
    {
        private readonly RecycleContext _dbContext;

        public NewsletterController(RecycleContext dbContext)
        {
            _dbContext = dbContext;
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribe([FromBody] SubscribeVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Checks the email is not already subscribed:
            if (await _dbContext.NewsletterSubscribers.AnyAsync(s => s.EmailAddress.Equals(model.EmailAddress)) == false)
            {
                NewsletterSubscriber subscriber = new NewsletterSubscriber()
                {
                    EmailAddress = model.EmailAddress
                };
                _dbContext.NewsletterSubscribers.Add(subscriber);
                await _dbContext.SaveChangesAsync();
            }
            return Json(new { Message = "Great! We will let you know about awesome jewelry!" });
        }

    }
}
