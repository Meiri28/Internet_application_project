using Recycle.App_Custom.Helpers.ViewModelHelpers;
using Recycle.Areas.Admin.ViewModels.Contacts;
using Recycle.Data;
using Recycle.Models;
using Recycle.Models.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recycle.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ContactsController : Controller
    {
        private readonly RecycleContext _dbContext;

        public ContactsController(RecycleContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: /Admin/Contacts
        public async Task<IActionResult> Index(IndexVM model)
        {
            List<Contact> contacts = await _dbContext.Contacts
                .Where(c => (model.Query == null || c.Name.Contains(model.Query) || c.EmailAddress.Contains(model.Query) || c.Subject.Contains(model.Query) || c.Body.Contains(model.Query)) &&
                            (model.Status == null || c.Status == model.Status.Value))
                .OrderByDescending(c => c.DateCreated)
                .ToListAsync();

            #region Pagination...

            Pagination pagination = new Pagination(contacts.Count, model.PageSize, model.Page);
            if (pagination.HasPagination())
            {
                if (pagination.PageSize.HasValue)
                {
                    contacts = contacts
                        .Skip(pagination.GetRecordsSkipped())
                        .Take(pagination.PageSize.Value)
                        .ToList();
                }
            }
            ViewData["Pagination"] = pagination;

            #endregion

            ViewData["ContactsModel"] = contacts;
            return View(model);
        }

        // GET: /Admin/Contacts/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            Contact contact = await _dbContext.Contacts.FirstOrDefaultAsync(c => c.Id == id);
            if (contact == null)
                return NotFound();
            else
                return View(contact);
        }

        // GET: /Admin/Contacts/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            Contact contact = await _dbContext.Contacts.FirstOrDefaultAsync(c => c.Id == id);
            if (contact == null)
                return NotFound();
            else
                return View(contact);
        }

        // POST: /Admin/Contacts/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete_POST(int id)
        {
            Contact contact = await _dbContext.Contacts.FindAsync(id);
            _dbContext.Contacts.Remove(contact);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Contacts/UpdateStatus/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, ContactStatus status)
        {
            Contact contact = await _dbContext.Contacts.FirstOrDefaultAsync(c => c.Id == id);
            if (contact == null)
                return NotFound();

            // Binds the view model:
            contact.Status = status;
            contact.DateLastModified = DateTime.Now;

            _dbContext.Contacts.Update(contact);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
