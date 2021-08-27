using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Recycle.Data;
using Recycle.Models;

namespace Recycle.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StoreCommentsController : Controller
    {
        private readonly RecycleContext _context;

        public StoreCommentsController(RecycleContext context)
        {
            _context = context;
        }

        // GET: StoreComments
        public async Task<IActionResult> Index()
        {
            var recycleContext = _context.StoreComment.Include(s => s.Store).Include(s => s.Writer);
            return View(await recycleContext.ToListAsync());
        }

        // GET: StoreComments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storeComment = await _context.StoreComment
                .Include(s => s.Store)
                .Include(s => s.Writer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (storeComment == null)
            {
                return NotFound();
            }

            return View(storeComment);
        }

        // GET: StoreComments/Create
        public IActionResult Create()
        {
            ViewData["StoreId"] = new SelectList(_context.Store, "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Email");
            return View();
        }

        // POST: StoreComments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,StoreId,body,CreatedAt,UpdatedAt")] StoreComment storeComment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(storeComment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StoreId"] = new SelectList(_context.Store, "Id", "Name", storeComment.StoreId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Email", storeComment.UserId);
            return View(storeComment);
        }

        // GET: StoreComments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storeComment = await _context.StoreComment.FindAsync(id);
            if (storeComment == null)
            {
                return NotFound();
            }
            ViewData["StoreId"] = new SelectList(_context.Store, "Id", "Name", storeComment.StoreId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Email", storeComment.UserId);
            return View(storeComment);
        }

        // POST: StoreComments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,StoreId,body,CreatedAt,UpdatedAt")] StoreComment storeComment)
        {
            if (id != storeComment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(storeComment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoreCommentExists(storeComment.Id))
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
            ViewData["StoreId"] = new SelectList(_context.Store, "Id", "Name", storeComment.StoreId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Email", storeComment.UserId);
            return View(storeComment);
        }

        // GET: StoreComments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storeComment = await _context.StoreComment
                .Include(s => s.Store)
                .Include(s => s.Writer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (storeComment == null)
            {
                return NotFound();
            }

            return View(storeComment);
        }

        // POST: StoreComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var storeComment = await _context.StoreComment.FindAsync(id);
            _context.StoreComment.Remove(storeComment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StoreCommentExists(int id)
        {
            return _context.StoreComment.Any(e => e.Id == id);
        }
    }
}
