using Recycle.App_Custom.ActionFilters;
using Recycle.App_Custom.Helpers.ViewModelHelpers;
using Recycle.Areas.Seller.ViewModels.Products;
using Recycle.Data;
using Recycle.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Recycle.Services;

namespace Recycle.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize(Roles = "Seller")]
    public class ProductsController : Controller
    {
        private const string IMAGES_FOLDER_PATH = "/files/images/products";

        private readonly RecycleContext _dbContext;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly UserIdentityService _userIdentity;

        public ProductsController(RecycleContext dbContext, IWebHostEnvironment hostEnvironment, IConfiguration configuration, UserIdentityService userIdentity)
        {
            _dbContext = dbContext;
            _hostEnvironment = hostEnvironment;
            _configuration = configuration;
            _userIdentity = userIdentity;
        }

        // GET: /Seller/Products
        public async Task<IActionResult> Index(IndexVM model)
        {
            int? userId = _userIdentity.GetCurrentId();
            if (userId == null)
                return NotFound();

            // user can craete only one store
            var Stores = await _dbContext.Stores
                .Where(b => (b.UserId == (int)userId))
                .Select(b => b.Id)
                .ToListAsync();

            if (Stores == null) 
                return NotFound();


            List<Product> products = await _dbContext.Products
                .Where(p => ((model.Query == null) || p.Name.Contains(model.Query) || p.Description.Contains(model.Query)) &&
                            ((model.CategoryId == null) || p.CategoryId == model.CategoryId) &&
                            ((model.TypeId == null) || p.TypeId == model.TypeId) &&
                            ((model.SaleId == null) || p.SaleId == model.SaleId) &&
                            ( p.StoreId == Stores[0] ) )
                .OrderByDescending(p => p.DateLastModified)
                .Include(p => p.Category).Include(p => p.Type).Include(p => p.Sale)
                .ToListAsync();

            #region Pagination...

            Pagination pagination = new Pagination(products.Count, model.PageSize, model.Page);
            if (pagination.HasPagination())
            {
                if (pagination.PageSize.HasValue)
                {
                    products = products
                        .Skip(pagination.GetRecordsSkipped())
                        .Take(pagination.PageSize.Value)
                        .ToList();
                }
            }
            ViewData["Pagination"] = pagination;

            #endregion

            ViewData["ProductsModel"] = products;
            ViewData["ProductCategories"] = new SelectList(_dbContext.ProductCategories, nameof(ProductCategory.Id), nameof(ProductCategory.Name));
            ViewData["ProductTypes"] = new SelectList(_dbContext.ProductTypes, nameof(ProductType.Id), nameof(ProductType.Name));
            ViewData["Sales"] = new SelectList(_dbContext.Sales, nameof(Sale.Id), nameof(Sale.Name));
            return View(model);
        }

        // GET: /Seller/Products/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            Product product = await _dbContext.Products
                .Include(p => p.Category).Include(p => p.Type).Include(p => p.Sale)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return NotFound();
            else
                return View(product);
        }

        // GET: /Seller/Products/Create
        public IActionResult Create()
        {
            int? userId = _userIdentity.GetCurrentId();
            if (userId == null)
                return NotFound();

            ViewData["ProductCategories"] = new SelectList(_dbContext.ProductCategories, nameof(ProductCategory.Id), nameof(ProductCategory.Name));
            ViewData["ProductTypes"] = new SelectList(_dbContext.ProductTypes, nameof(ProductType.Id), nameof(ProductType.Name));
            ViewData["Sales"] = new SelectList(_dbContext.Sales, nameof(Sale.Id), nameof(Sale.Name));
            ViewData["Stores"] = new SelectList(_dbContext.Stores.Where(s => s.UserId == userId), nameof(Store.Id), nameof(Store.Name));
            return View(new CreateVM()
            {
                IsAvailable = true,
            });
        }

        // POST: /Seller/Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVM model)
        {

            int? userId = _userIdentity.GetCurrentId();
            if (userId == null)
                return NotFound();

            List<Store> Stores = await _dbContext.Stores
                .Where(b => ((b.UserId == null) || (b.UserId == (int)userId)))
                .OrderBy(b => b.Name)
                .ToListAsync();

            if (!ModelState.IsValid)
            {
                ViewData["ProductCategories"] = new SelectList(_dbContext.ProductCategories, nameof(ProductCategory.Id), nameof(ProductCategory.Name), model.CategoryId);
                ViewData["ProductTypes"] = new SelectList(_dbContext.ProductTypes, nameof(ProductType.Id), nameof(ProductType.Name), model.TypeId);
                ViewData["Sales"] = new SelectList(_dbContext.Sales, nameof(Sale.Id), nameof(Sale.Name), model.SaleId);
                ViewData["Stores"] = new SelectList(Stores, nameof(Store.Id), nameof(Store.Name), model.StoreId);
                return View(model);
            }

            // Saves the product image:
            string imagePath = await SaveImage(model.ImageFile);

            Product product = new Product()
            {
                Name = model.Name,
                Description = model.Description,
                ImagePath = imagePath,
                Price = model.Price,
                IsAvailable = model.IsAvailable,
                CategoryId = model.CategoryId,
                TypeId = model.TypeId,
                SaleId = model.SaleId,
                StoreId = model.StoreId
            };
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }

        // GET: /Seller/Products/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            Product product = await _dbContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            else
            {
                ViewData["ProductCategories"] = new SelectList(_dbContext.ProductCategories, nameof(ProductCategory.Id), nameof(ProductCategory.Name), product.CategoryId);
                ViewData["ProductTypes"] = new SelectList(_dbContext.ProductTypes, nameof(ProductType.Id), nameof(ProductType.Name), product.TypeId);
                ViewData["Sales"] = new SelectList(_dbContext.Sales, nameof(Sale.Id), nameof(Sale.Name), product.SaleId);
                ViewData["Stores"] = new SelectList(_dbContext.Stores, nameof(Store.Id), nameof(Store.Name), product.StoreId);
                return View(new EditVM()
                {
                    Id = product.Id,
                    CurrentName = product.Name,
                    Name = product.Name,
                    Description = product.Description,
                    ImagePath = product.ImagePath,
                    Price = product.Price,
                    IsAvailable = product.IsAvailable,
                    CategoryId = product.CategoryId,
                    TypeId = product.TypeId,
                    SaleId = product.SaleId,
                    StoreId = product.StoreId
                });
            }
        }

        // POST: /Seller/Products/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditVM model)
        {
            if (id != model.Id)
                return NotFound();

            Product product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["ProductCategories"] = new SelectList(_dbContext.ProductCategories, nameof(ProductCategory.Id), nameof(ProductCategory.Name), product.CategoryId);
                ViewData["ProductTypes"] = new SelectList(_dbContext.ProductTypes, nameof(ProductType.Id), nameof(ProductType.Name), product.TypeId);
                ViewData["Sales"] = new SelectList(_dbContext.Sales, nameof(Sale.Id), nameof(Sale.Name), product.SaleId);
                return View(model);
            }

            // Checks if the image was changed, in order to update files:
            if (model.ImageFile != null)
            {
                DeleteImage(product.ImagePath);
                model.ImagePath = await SaveImage(model.ImageFile);
            }
            else
            {
                model.ImagePath = product.ImagePath;
            }

            // Binds the view model:
            product.Name = model.Name;
            product.Description = model.Description;
            product.ImagePath = model.ImagePath;
            product.Price = model.Price;
            product.IsAvailable = model.IsAvailable;
            product.CategoryId = model.CategoryId;
            product.TypeId = model.TypeId;
            product.SaleId = model.SaleId;
            product.DateLastModified = DateTime.Now;

            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Seller/Products/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            Product product = await _dbContext.Products
                .Include(p => p.Category).Include(p => p.Type).Include(p => p.Sale)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return NotFound();
            else
                return View(product);
        }

        // POST: /Seller/Products/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete_POST(int id)
        {
            Product product = await _dbContext.Products.FindAsync(id);
            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();

            // Deletes the product image:
            DeleteImage(product.ImagePath);

            return RedirectToAction(nameof(Index));
        }

        #region AJAX Actions

        [AjaxOnly]
        public async Task<JsonResult> CheckNameAvailability(string name)
        {
            bool isNameAvailable = (await _dbContext.Products.AnyAsync(p => p.Name.Equals(name)) == false);
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
                bool isNameAvailable = (await _dbContext.Products.AnyAsync(p => p.Name.Equals(name)) == false);
                return Json(isNameAvailable);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Saves the specified product image file to its folder, and returns the relative image path saved.
        /// </summary>
        /// <param name="imageFile">The product image file.</param>
        /// <returns>Returns the relative image path saved.</returns>
        private async Task<string> SaveImage(IFormFile imageFile)
        {
            // Generates a unique file name by the current time (year-month-day-hour-minute-second-millisecond):
            string fileName = (DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(imageFile.FileName));

            string relativeImagePath = $"{IMAGES_FOLDER_PATH}/{fileName}";
            string absoluteImagePath = Path.Combine(_hostEnvironment.WebRootPath, relativeImagePath.TrimStart('/'));

            using (FileStream fileStream = new FileStream(absoluteImagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            return relativeImagePath;
        }

        /// <summary>
        /// Deletes the specified product image path (relative path).
        /// </summary>
        /// <param name="imagePath">The image relative path to delete.</param>
        private void DeleteImage(string imagePath)
        {
            string absoluteImagePath = Path.Combine(_hostEnvironment.WebRootPath, imagePath.TrimStart('/'));
            if (System.IO.File.Exists(absoluteImagePath))
            {
                System.IO.File.Delete(absoluteImagePath);
            }
        }

        #endregion

    }
}
