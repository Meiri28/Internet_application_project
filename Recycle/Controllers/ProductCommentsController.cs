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
    public class ProductCommentsController : Controller
    {
        private readonly RecycleContext _context;

        public ProductCommentsController(RecycleContext context)
        {
            _context = context;
        }

        // GET: ProductComments
        public async Task<IActionResult> Index()
        {
            var recycleContext = _context.ProductsComment.Include(p => p.Product).Include(p => p.Writer);
            return View(await recycleContext.ToListAsync());
        }

        // GET: ProductComments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productComment = await _context.ProductsComment
                .Include(p => p.Product)
                .Include(p => p.Writer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productComment == null)
            {
                return NotFound();
            }

            return View(productComment);
        }

        // GET: ProductComments/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "Color");
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Email");
            return View();
        }

        // POST: ProductComments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,ProductId,body,CreatedAt,UpdatedAt")] ProductComment productComment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productComment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "Color", productComment.ProductId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Email", productComment.UserId);
            return View(productComment);
        }

        // GET: ProductComments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productComment = await _context.ProductsComment.FindAsync(id);
            if (productComment == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "Color", productComment.ProductId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Email", productComment.UserId);
            return View(productComment);
        }

        // POST: ProductComments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,ProductId,body,CreatedAt,UpdatedAt")] ProductComment productComment)
        {
            if (id != productComment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productComment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductCommentExists(productComment.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "Color", productComment.ProductId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Email", productComment.UserId);
            return View(productComment);
        }

        // GET: ProductComments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productComment = await _context.ProductsComment
                .Include(p => p.Product)
                .Include(p => p.Writer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productComment == null)
            {
                return NotFound();
            }

            return View(productComment);
        }

        // POST: ProductComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productComment = await _context.ProductsComment.FindAsync(id);
            _context.ProductsComment.Remove(productComment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductCommentExists(int id)
        {
            return _context.ProductsComment.Any(e => e.Id == id);
        }
    }
}
