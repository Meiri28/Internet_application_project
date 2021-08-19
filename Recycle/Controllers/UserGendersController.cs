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
    public class UserGendersController : Controller
    {
        private readonly RecycleContext _context;

        public UserGendersController(RecycleContext context)
        {
            _context = context;
        }

        // GET: UserGenders
        public async Task<IActionResult> Index()
        {
            return View(await _context.UserGender.ToListAsync());
        }

        // GET: UserGenders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userGender = await _context.UserGender
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userGender == null)
            {
                return NotFound();
            }

            return View(userGender);
        }

        // GET: UserGenders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserGenders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title")] UserGender userGender)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userGender);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userGender);
        }

        // GET: UserGenders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userGender = await _context.UserGender.FindAsync(id);
            if (userGender == null)
            {
                return NotFound();
            }
            return View(userGender);
        }

        // POST: UserGenders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title")] UserGender userGender)
        {
            if (id != userGender.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userGender);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserGenderExists(userGender.Id))
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
            return View(userGender);
        }

        // GET: UserGenders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userGender = await _context.UserGender
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userGender == null)
            {
                return NotFound();
            }

            return View(userGender);
        }

        // POST: UserGenders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userGender = await _context.UserGender.FindAsync(id);
            _context.UserGender.Remove(userGender);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserGenderExists(int id)
        {
            return _context.UserGender.Any(e => e.Id == id);
        }
    }
}
