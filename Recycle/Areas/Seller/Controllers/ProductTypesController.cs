using Recycle.App_Custom.ActionFilters;
using Recycle.App_Custom.Helpers.ViewModelHelpers;
using Recycle.Areas.Seller.ViewModels.ProductTypes;
using Recycle.Data;
using Recycle.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recycle.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize(Roles = "Seller")]
    public class ProductTypesController : Controller
    {
        private readonly RecycleContext _dbContext;

        public ProductTypesController(RecycleContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: /Seller/ProductTypes
        public async Task<IActionResult> Index(IndexVM model)
        {
            List<ProductType> types = await _dbContext.ProductTypes
                .Where(t => (model.Query == null) || t.Name.Contains(model.Query))
                .OrderBy(t => t.Name)
                .ToListAsync();

            #region Pagination...

            Pagination pagination = new Pagination(types.Count, model.PageSize, model.Page);
            if (pagination.HasPagination())
            {
                if (pagination.PageSize.HasValue)
                {
                    types = types
                        .Skip(pagination.GetRecordsSkipped())
                        .Take(pagination.PageSize.Value)
                        .ToList();
                }
            }
            ViewData["Pagination"] = pagination;

            #endregion

            ViewData["ProductTypesModel"] = types;
            return View(model);
        }

        // GET: /Seller/ProductTypes/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            ProductType types = await _dbContext.ProductTypes.FirstOrDefaultAsync(t => t.Id == id);
            if (types == null)
                return NotFound();
            else
                return View(types);
        }

        // GET: /Seller/ProductTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Seller/ProductTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            ProductType type = new ProductType()
            {
                Name = model.Name
            };
            _dbContext.ProductTypes.Add(type);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Seller/ProductTypes/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            ProductType type = await _dbContext.ProductTypes.FindAsync(id);
            if (type == null)
                return NotFound();
            else
                return View(new EditVM()
                {
                    Id = type.Id,
                    CurrentName = type.Name,
                    Name = type.Name
                });
        }

        // POST: /Seller/ProductTypes/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditVM model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            ProductType type = await _dbContext.ProductTypes.FirstOrDefaultAsync(c => c.Id == id);
            if (type == null)
                return NotFound();

            // Binds the view model:
            type.Name = model.Name;
            type.DateLastModified = DateTime.Now;

            _dbContext.ProductTypes.Update(type);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Seller/ProductTypes/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            ProductType type = await _dbContext.ProductTypes.FirstOrDefaultAsync(c => c.Id == id);
            if (type == null)
                return NotFound();
            else
                return View(type);
        }

        // POST: /Seller/ProductTypes/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete_POST(int id)
        {
            ProductType type = await _dbContext.ProductTypes.FindAsync(id);
            _dbContext.ProductTypes.Remove(type);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #region AJAX Actions

        [AjaxOnly]
        public async Task<JsonResult> CheckNameAvailability(string name)
        {
            bool isNameAvailable = (await _dbContext.ProductTypes.AnyAsync(t => t.Name.Equals(name)) == false);
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
                bool isNameAvailable = (await _dbContext.ProductTypes.AnyAsync(t => t.Name.Equals(name)) == false);
                return Json(isNameAvailable);
            }
        }

        #endregion

    }
}
