using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Recycle.Data;
using Recycle.Models;

namespace Recycle.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class StoresController : Controller
    {
        private readonly RecycleContext _context;

        public StoresController(RecycleContext context)
        {
            _context = context;
        }

        // GET: Stores
        public async Task<IActionResult> Index()
        {
            var recycleContext = _context.Store.Include(s => s.Owner);
            return View(await recycleContext.ToListAsync());
        }

        // GET: Stores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var store = await _context.Store
                .Include(s => s.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> MyStore()
        {
            int store_id = _context.Store.Where(s=>s.UserId == _context.User.Where(u => u.Email == HttpContext.User.FindFirst(ClaimTypes.Email).Value).First().Id).First().Id;
            return RedirectToAction(nameof(Edit), "Stores", new RouteValueDictionary( new { Id = store_id }));
        }

        // GET: Stores/Create
        public IActionResult CreateAdmin()
        {
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Email");
            return View();
        }

        // POST: Stores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdmin([Bind("Id,UserId,Name,Rate,IsActive,CreatedAt,UpdatedAt")] Store store)
        {
            if (ModelState.IsValid)
            {
                _context.Add(store);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Email", store.UserId);
            return View(store);
        }

        // GET: Stores/Create
        [Authorize(Roles = "Buyer")]
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Email");
            return View();
        }

        // POST: Stores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Buyer")]
        public async Task<IActionResult> Create([Bind("Id,Name,CreatedAt")] Store store)
        {
            store.Rate = 1;
            store.IsActive = true;
            store.UpdatedAt = store.CreatedAt;
            store.UserId = _context.User.Where(u => u.Email == HttpContext.User.FindFirst(ClaimTypes.Email).Value).First().Id;
            if (ModelState.IsValid)
            {
                _context.Add(store);
                await _context.SaveChangesAsync();
                //TODO: change buyer to seller
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Email", store.UserId);
            return View(store);
        }

        // GET: Stores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var store = await _context.Store.FindAsync(id);
            if (store == null)
            {
                return NotFound();
            }
            if (!IsOwnerOfStore(id.GetValueOrDefault(0)))
            {
                // TODO: return access denied
                // try to edit other user store
                return NotFound();
            }

            ViewData["UserId"] = new SelectList(_context.User, "Id", "Email", store.UserId);
            return View(store);
        }

        // POST: Stores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, String Name)
        {
            if (!IsOwnerOfStore(id))
            {
                // TODO: return access denied
                // try to edit other user store
                return NotFound();
            }

            Store store = GetUserStore();
            store.Name = Name;
            store.UpdatedAt = DateTime.Now;

            if (id != store.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(store);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoreExists(store.Id))
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
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Email", store.UserId);
            return View(store);
        }

        // GET: Stores/Edit/5
        public async Task<IActionResult> EditAdmin(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var store = await _context.Store.FindAsync(id);
            if (store == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Email", store.UserId);
            return View(store);
        }

        // POST: Stores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAdmin(int id, [Bind("Id,UserId,Name,Rate,IsActive,CreatedAt,UpdatedAt")] Store store)
        {
            if (id != store.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(store);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoreExists(store.Id))
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
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Email", store.UserId);
            return View(store);
        }

        // GET: Stores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var store = await _context.Store
                .Include(s => s.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

        // POST: Stores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var store = await _context.Store.FindAsync(id);
            _context.Store.Remove(store);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StoreExists(int id)
        {
            return _context.Store.Any(e => e.Id == id);
        }

        private bool IsOwnerOfStore(int Store_id_to_check)
        {
            Store store = GetUserStore();
            return store.Id== Store_id_to_check;
        }

        private Store GetUserStore()
        {
            return _context.Store.Where(s => s.UserId == _context.User.Where(u => u.Email == HttpContext.User.FindFirst(ClaimTypes.Email).Value).First().Id).First();
        }
    }
}
