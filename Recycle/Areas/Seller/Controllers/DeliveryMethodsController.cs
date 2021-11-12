﻿using Recycle.App_Custom.ActionFilters;
using Recycle.App_Custom.Helpers.ViewModelHelpers;
using Recycle.Areas.Seller.ViewModels.DeliveryMethods;
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
    public class DeliveryMethodsController : Controller
    {
        private readonly RecycleContext _dbContext;

        public DeliveryMethodsController(RecycleContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: /Seller/DeliveryMethods
        public async Task<IActionResult> Index(IndexVM model)
        {
            List<DeliveryMethod> deliveryMethods = await _dbContext.DeliveryMethods
                .Where(d => (model.Query == null) || d.Name.Contains(model.Query) || d.Description.Contains(model.Query))
                .OrderBy(d => d.Price)
                .ToListAsync();

            #region Pagination...

            Pagination pagination = new Pagination(deliveryMethods.Count, model.PageSize, model.Page);
            if (pagination.HasPagination())
            {
                if (pagination.PageSize.HasValue)
                {
                    deliveryMethods = deliveryMethods
                        .Skip(pagination.GetRecordsSkipped())
                        .Take(pagination.PageSize.Value)
                        .ToList();
                }
            }
            ViewData["Pagination"] = pagination;

            #endregion

            ViewData["DeliveryMethodsModel"] = deliveryMethods;
            return View(model);
        }

        // GET: /Seller/DeliveryMethods/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            DeliveryMethod deliveryMethod = await _dbContext.DeliveryMethods.FirstOrDefaultAsync(d => d.Id == id);
            if (deliveryMethod == null)
                return NotFound();
            else
                return View(deliveryMethod);
        }

        // GET: /Seller/DeliveryMethods/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Seller/DeliveryMethods/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            DeliveryMethod deliveryMethod = new DeliveryMethod()
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price
            };
            _dbContext.DeliveryMethods.Add(deliveryMethod);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Seller/DeliveryMethods/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            DeliveryMethod deliveryMethod = await _dbContext.DeliveryMethods.FindAsync(id);
            if (deliveryMethod == null)
                return NotFound();
            else
                return View(new EditVM()
                {
                    Id = deliveryMethod.Id,
                    CurrentName = deliveryMethod.Name,
                    Name = deliveryMethod.Name,
                    Description = deliveryMethod.Description,
                    Price = deliveryMethod.Price
                });
        }

        // POST: /Seller/DeliveryMethods/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditVM model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            DeliveryMethod deliveryMethod = await _dbContext.DeliveryMethods.FirstOrDefaultAsync(d => d.Id == id);
            if (deliveryMethod == null)
                return NotFound();

            // Binds the view model:
            deliveryMethod.Name = model.Name;
            deliveryMethod.Description = model.Description;
            deliveryMethod.Price = model.Price;
            deliveryMethod.DateLastModified = DateTime.Now;

            _dbContext.DeliveryMethods.Update(deliveryMethod);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Seller/DeliveryMethods/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            DeliveryMethod deliveryMethod = await _dbContext.DeliveryMethods.FirstOrDefaultAsync(d => d.Id == id);
            if (deliveryMethod == null)
                return NotFound();
            else
                return View(deliveryMethod);
        }

        // POST: /Seller/DeliveryMethods/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete_POST(int id)
        {
            DeliveryMethod deliveryMethod = await _dbContext.DeliveryMethods.FindAsync(id);
            _dbContext.DeliveryMethods.Remove(deliveryMethod);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #region AJAX Actions

        [AjaxOnly]
        public async Task<JsonResult> CheckNameAvailability(string name)
        {
            bool isNameAvailable = (await _dbContext.DeliveryMethods.AnyAsync(d => d.Name.Equals(name)) == false);
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
                bool isNameAvailable = (await _dbContext.DeliveryMethods.AnyAsync(d => d.Name.Equals(name)) == false);
                return Json(isNameAvailable);
            }
        }

        #endregion

    }
}
