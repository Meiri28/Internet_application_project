﻿using Recycle.App_Custom.ActionFilters;
using Recycle.App_Custom.Helpers.ViewModelHelpers;
using Recycle.Areas.Seller.ViewModels.Sales;
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
    public class SalesController : Controller
    {
        private readonly RecycleContext _dbContext;

        public SalesController(RecycleContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: /Seller/Sales
        public async Task<IActionResult> Index(IndexVM model)
        {
            List<Sale> sales = await _dbContext.Sales
                .Where(s => ((model.Query == null) || s.Name.Contains(model.Query)) &&
                            ((model.DuringStart == null) || (s.DateEnd.HasValue ? (model.DuringStart >= s.DateStart && model.DuringStart <= s.DateEnd) : (model.DuringStart >= s.DateStart))) &&
                            ((model.DuringStart == null) || (s.DateEnd == null) || model.DuringEnd <= s.DateEnd))
                .OrderByDescending(s => s.DateLastModified)
                .ToListAsync();

            #region Pagination...

            Pagination pagination = new Pagination(sales.Count, model.PageSize, model.Page);
            if (pagination.HasPagination())
            {
                if (pagination.PageSize.HasValue)
                {
                    sales = sales
                        .Skip(pagination.GetRecordsSkipped())
                        .Take(pagination.PageSize.Value)
                        .ToList();
                }
            }
            ViewData["Pagination"] = pagination;

            #endregion

            ViewData["SalesModel"] = sales;
            return View(model);
        }

        // GET: /Seller/Sales/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            Sale sale = await _dbContext.Sales.FirstOrDefaultAsync(s => s.Id == id);
            if (sale == null)
                return NotFound();
            else
                return View(sale);
        }

        // GET: /Seller/Sales/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Seller/Sales/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            Sale sale = new Sale()
            {
                Name = model.Name,
                DiscountRate = model.DiscountRate,
                DateStart = model.DateStart,
                DateEnd = model.DateEnd
            };
            _dbContext.Sales.Add(sale);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Seller/Sales/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            Sale sale = await _dbContext.Sales.FindAsync(id);
            if (sale == null)
                return NotFound();
            else
                return View(new EditVM()
                {
                    Id = sale.Id,
                    CurrentName = sale.Name,
                    Name = sale.Name,
                    DiscountRate = sale.DiscountRate,
                    DateStart = sale.DateStart,
                    DateEnd = sale.DateEnd
                });
        }

        // POST: /Seller/Sales/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditVM model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            Sale sale = await _dbContext.Sales.FirstOrDefaultAsync(s => s.Id == id);
            if (sale == null)
                return NotFound();

            // Binds the view model:
            sale.Name = model.Name;
            sale.DiscountRate = model.DiscountRate;
            sale.DateStart = model.DateStart;
            sale.DateEnd = model.DateEnd;
            sale.DateLastModified = DateTime.Now;

            _dbContext.Sales.Update(sale);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Seller/Sales/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            Sale sale = await _dbContext.Sales.FirstOrDefaultAsync(s => s.Id == id);
            if (sale == null)
                return NotFound();
            else
                return View(sale);
        }

        // POST: /Seller/Sales/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete_POST(int id)
        {
            Sale sale = await _dbContext.Sales.FindAsync(id);
            _dbContext.Sales.Remove(sale);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #region AJAX Actions

        [AjaxOnly]
        public async Task<JsonResult> CheckNameAvailability(string name)
        {
            bool isNameAvailable = (await _dbContext.Sales.AnyAsync(s => s.Name.Equals(name)) == false);
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
                bool isNameAvailable = (await _dbContext.Sales.AnyAsync(s => s.Name.Equals(name)) == false);
                return Json(isNameAvailable);
            }
        }

        #endregion

    }
}
