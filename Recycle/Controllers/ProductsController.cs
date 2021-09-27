using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Recycle.Data;
using Recycle.Models;

namespace Recycle.Controllers
{
    public class ProductsController : Controller
    {
        private readonly RecycleContext _context;

        public ProductsController(RecycleContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var recycleContext = _context.Product.Include(p => p.Store).Include(p => p.Pictures);
            return View(await recycleContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.Store)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["StoreId"] = new SelectList(_context.Store, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StoreId,ItemName,ItemDesc,Size,Amount,Price,Color,Pictures,VideoURL,IsActive,CreatedAt,UpdatedAt")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StoreId"] = new SelectList(_context.Store, "Id", "Name", product.StoreId);
            return View(product);
        }

        // GET: Products/Create
        public IActionResult AddProduct()
        {
            ViewData["StoreId"] = new SelectList(_context.Store, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> AddProduct([Bind("Id,ItemName,ItemDesc,Size,Amount,Price,Color,PictursFiles,VideoFile")] Product product, String hashtaginput)
        {
            product.UpdatedAt = product.CreatedAt;
            product.IsActive = true;
            product.StoreId = _context.Store.Where(s => s.UserId == _context.User.Where(u => u.Email == HttpContext.User.FindFirst(ClaimTypes.Email).Value).First().Id).First().Id;
            if (ModelState.IsValid)
            {
                product.Pictures = new List<ProductImage>();
                if(product.PictursFiles != null) { 
                    foreach (IFormFile image in product.PictursFiles){
                        //store the image
                        using ( MemoryStream ms= new MemoryStream())
                        {
                            image.CopyTo(ms);
                            ProductImage p = new ProductImage();
                            p.Product = product;
                            p.Data = ms.ToArray();
                            product.Pictures.Add(p);
                        }
                    }
                }
                if(product.VideoFile != null)
                { 
                    //store the video
                    using (MemoryStream ms = new MemoryStream())
                    {
                        product.VideoFile.CopyTo(ms);
                        product.Video = ms.ToArray();
                    }
                }

                product.Hashtags = new List<Hashtag>();
                foreach (String hashtagTital in hashtaginput.Split(','))
                {
                    Hashtag hashtag = _context.Hashtag.Find(hashtagTital);
                    if (hashtag == null)
                    {
                        hashtag = new Hashtag();
                        hashtag.Title = hashtagTital;
                    }
                    
                    product.Hashtags.Add(hashtag);
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StoreId"] = new SelectList(_context.Store, "Id", "Name", product.StoreId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.Include(P => P.Hashtags).FirstAsync(product => product.Id == (int)id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["StoreId"] = new SelectList(_context.Store, "Id", "Name", product.StoreId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StoreId,ItemName,ItemDesc,Size,Amount,Price,Color,Pictures,VideoURL,IsActive")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.UpdatedAt = DateTime.Now;
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["StoreId"] = new SelectList(_context.Store, "Id", "Name", product.StoreId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.Store)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
    }
}
