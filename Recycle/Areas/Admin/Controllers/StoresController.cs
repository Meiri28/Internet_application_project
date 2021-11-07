using Recycle.App_Custom.ActionFilters;
using Recycle.App_Custom.Helpers.ViewModelHelpers;
using Recycle.Areas.Admin.ViewModels.Stores;
using Recycle.Data;
using Recycle.Models;
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
    public class StoresController : Controller
    {
        private readonly RecycleContext _dbContext;

        public StoresController(RecycleContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: /Admin/Stores
        public async Task<IActionResult> Index(IndexVM model)
        {
            List<Store> Stores = await _dbContext.Stores
                .Where(b => (model.Query == null) || b.Name.Contains(model.Query))
                .OrderBy(b => b.Name)
                .ToListAsync();

            #region Pagination...

            Pagination pagination = new Pagination(Stores.Count, model.PageSize, model.Page);
            if (pagination.HasPagination())
            {
                if (pagination.PageSize.HasValue)
                {
                    Stores = Stores
                        .Skip(pagination.GetRecordsSkipped())
                        .Take(pagination.PageSize.Value)
                        .ToList();
                }
            }
            ViewData["Pagination"] = pagination;

            #endregion

            ViewData["StoresModel"] = Stores;
            return View(model);
        }

        // GET: /Admin/Stores/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            Store Store = await _dbContext.Stores.FirstOrDefaultAsync(b => b.Id == id);
            if (Store == null)
                return NotFound();
            else
                return View(Store);
        }

        // GET: /Admin/Stores/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/Stores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            Store Store = new Store()
            {
                Name = model.Name,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                OpeningHours = model.OpeningHours,
                LocationLatitude = model.LocationLatitude,
                LocationLongitude = model.LocationLongitude
            };
            _dbContext.Stores.Add(Store);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Stores/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            Store Store = await _dbContext.Stores.FindAsync(id);
            if (Store == null)
                return NotFound();
            else
                return View(new EditVM()
                {
                    Id = Store.Id,
                    CurrentName = Store.Name,
                    Name = Store.Name,
                    Address = Store.Address,
                    PhoneNumber = Store.PhoneNumber,
                    OpeningHours = Store.OpeningHours,
                    LocationLatitude = Store.LocationLatitude,
                    LocationLongitude = Store.LocationLongitude
                });
        }

        // POST: /Admin/Stores/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditVM model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            Store Store = await _dbContext.Stores.FirstOrDefaultAsync(b => b.Id == id);
            if (Store == null)
                return NotFound();

            // Binds the view model:
            Store.Name = model.Name;
            Store.Address = model.Address;
            Store.PhoneNumber = model.PhoneNumber;
            Store.OpeningHours = model.OpeningHours;
            Store.LocationLatitude = model.LocationLatitude;
            Store.LocationLongitude = model.LocationLongitude;
            Store.DateLastModified = DateTime.Now;

            _dbContext.Stores.Update(Store);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Stores/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            Store Store = await _dbContext.Stores.FirstOrDefaultAsync(b => b.Id == id);
            if (Store == null)
                return NotFound();
            else
                return View(Store);
        }

        // POST: /Admin/Stores/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete_POST(int id)
        {
            Store Store = await _dbContext.Stores.FindAsync(id);
            _dbContext.Stores.Remove(Store);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #region AJAX Actions

        [AjaxOnly]
        public async Task<JsonResult> CheckNameAvailability(string name)
        {
            bool isNameAvailable = (await _dbContext.Stores.AnyAsync(b => b.Name.Equals(name)) == false);
            return Json(isNameAvailable);
        }

        [AjaxOnly]
        public async Task<JsonResult> CheckNameEditAvailability(string name, string currentName)
        {
            // Checks if the name did not change in the edit:
            if (string.Equals(name, currentName, StringComparison.OrdinalIgnoreCase))
            {
                return Json(true);
            }
            // Otherwise, name was changed so checks availability:
            else
            {
                bool isNameAvailable = (await _dbContext.Stores.AnyAsync(b => b.Name.Equals(name)) == false);
                return Json(isNameAvailable);
            }
        }

        #endregion

    }
}
