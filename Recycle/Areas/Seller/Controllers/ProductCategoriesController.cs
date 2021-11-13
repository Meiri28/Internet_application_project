using Recycle.App_Custom.ActionFilters;
using Recycle.App_Custom.Helpers.ViewModelHelpers;
using Recycle.Areas.Seller.ViewModels.ProductCategories;
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
    public class ProductCategoriesController : Controller
    {
        private readonly RecycleContext _dbContext;

        public ProductCategoriesController(RecycleContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: /Seller/ProductCategories
        public async Task<IActionResult> Index(IndexVM model)
        {
            List<ProductCategory> categories = await _dbContext.ProductCategories
                .Where(c => (model.Query == null) || c.Name.Contains(model.Query))
                .OrderBy(c => c.Name)
                .ToListAsync();

            #region Pagination...

            Pagination pagination = new Pagination(categories.Count, model.PageSize, model.Page);
            if (pagination.HasPagination())
            {
                if (pagination.PageSize.HasValue)
                {
                    categories = categories
                        .Skip(pagination.GetRecordsSkipped())
                        .Take(pagination.PageSize.Value)
                        .ToList();
                }
            }
            ViewData["Pagination"] = pagination;

            #endregion

            ViewData["ProductCategoriesModel"] = categories;
            return View(model);
        }

        // GET: /Seller/ProductCategories/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            ProductCategory category = await _dbContext.ProductCategories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
                return NotFound();
            else
                return View(category);
        }

        // GET: /Seller/ProductCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Seller/ProductCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            ProductCategory category = new ProductCategory()
            {
                Name = model.Name
            };
            _dbContext.ProductCategories.Add(category);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Seller/ProductCategories/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            ProductCategory category = await _dbContext.ProductCategories.FindAsync(id);
            if (category == null)
                return NotFound();
            else
                return View(new EditVM()
                {
                    Id = category.Id,
                    CurrentName = category.Name,
                    Name = category.Name
                });
        }

        // POST: /Seller/ProductCategories/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditVM model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            ProductCategory category = await _dbContext.ProductCategories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
                return NotFound();

            // Binds the view model:
            category.Name = model.Name;
            category.DateLastModified = DateTime.Now;

            _dbContext.ProductCategories.Update(category);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Seller/ProductCategories/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            ProductCategory category = await _dbContext.ProductCategories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
                return NotFound();
            else
                return View(category);
        }

        // POST: /Seller/ProductCategories/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete_POST(int id)
        {
            ProductCategory category = await _dbContext.ProductCategories.FindAsync(id);
            _dbContext.ProductCategories.Remove(category);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #region AJAX Actions

        [AjaxOnly]
        public async Task<JsonResult> CheckNameAvailability(string name)
        {
            bool isNameAvailable = (await _dbContext.ProductCategories.AnyAsync(c => c.Name.Equals(name)) == false);
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
                bool isNameAvailable = (await _dbContext.ProductCategories.AnyAsync(c => c.Name.Equals(name)) == false);
                return Json(isNameAvailable);
            }
        }

        #endregion

    }
}
