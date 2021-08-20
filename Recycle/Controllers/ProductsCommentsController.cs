using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Recycle.Data;
using Recycle.Models;

namespace Recycle.Controllers
{
    public class ProductsCommentsController : Controller
    {
        private readonly RecycleContext _context;

        public ProductsCommentsController(RecycleContext context)
        {
            _context = context;
        }

        // GET: ProductsComments
        public async Task<IActionResult> Index()
        {
            var recycleContext = _context.ProductsComment.Include(p => p.Products).Include(p => p.Writer);
            return View(await recycleContext.ToListAsync());
        }

        // GET: ProductsComments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productsComment = await _context.ProductsComment
                .Include(p => p.Products)
                .Include(p => p.Writer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productsComment == null)
            {
                return NotFound();
            }

            return View(productsComment);
        }

        // GET: ProductsComments/Create
        public IActionResult Create()
        {
            ViewData["ProductsId"] = new SelectList(_context.Set<Products>(), "Id", "Color");
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Email");
            return View();
        }

        // POST: ProductsComments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,ProductsId,body,CreatedAt,UpdatedAt")] ProductsComment productsComment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productsComment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductsId"] = new SelectList(_context.Set<Products>(), "Id", "Color", productsComment.ProductsId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Email", productsComment.UserId);
            return View(productsComment);
        }

        // GET: ProductsComments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productsComment = await _context.ProductsComment.FindAsync(id);
            if (productsComment == null)
            {
                return NotFound();
            }
            ViewData["ProductsId"] = new SelectList(_context.Set<Products>(), "Id", "Color", productsComment.ProductsId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Email", productsComment.UserId);
            return View(productsComment);
        }

        // POST: ProductsComments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,ProductsId,body,CreatedAt,UpdatedAt")] ProductsComment productsComment)
        {
            if (id != productsComment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productsComment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductsCommentExists(productsComment.Id))
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
            ViewData["ProductsId"] = new SelectList(_context.Set<Products>(), "Id", "Color", productsComment.ProductsId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Email", productsComment.UserId);
            return View(productsComment);
        }

        // GET: ProductsComments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productsComment = await _context.ProductsComment
                .Include(p => p.Products)
                .Include(p => p.Writer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productsComment == null)
            {
                return NotFound();
            }

            return View(productsComment);
        }

        // POST: ProductsComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productsComment = await _context.ProductsComment.FindAsync(id);
            _context.ProductsComment.Remove(productsComment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductsCommentExists(int id)
        {
            return _context.ProductsComment.Any(e => e.Id == id);
        }
    }
}
