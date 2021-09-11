using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Recycle.Data;
using Recycle.Models;

namespace Recycle.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly RecycleContext _context;

        public TransactionsController(RecycleContext context)
        {
            _context = context;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            var recycleContext = _context.Transaction.Include(t => t.Product);
            return View(await recycleContext.ToListAsync());
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transaction
                .Include(t => t.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transactions/Create
        public IActionResult Create(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            Product product = _context.Product.Where(p => p.Id == id).FirstOrDefault();
            if(product == null)
            {
                return NotFound();
            }
            ViewBag.product = product;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, uint Amount)
        {
            var linq = from Product in _context.Product
                       join Store in _context.Store on Product.Store.Id equals Store.Id
                       join user in _context.User on Store.UserId equals user.Id
                       where id == Product.Id
                       select new { product =  Product, seller = user};
            Product product = linq.First().product;
            User buyer = _context.User.Where(u => u.Email == HttpContext.User.FindFirst(ClaimTypes.Email).Value).First();
            if (Amount * product.Price > buyer.Balance)
            {
                //TODO: dont have meony
                return RedirectToAction(nameof(Index));
            }
            if (product.Amount < Amount)
            {
                //TODO: dont have products
                return RedirectToAction(nameof(Index));
            }
            if(linq.First().seller.Id == buyer.Id)
            {
                return RedirectToAction(nameof(Index));
            }

            Transaction transaction = new Transaction();
            transaction.Amount = Amount;
            transaction.FromUserID = buyer.Id;
            transaction.ToUserID = linq.First().seller.Id;
            transaction.PriceForOneProduct = product.Price;
            transaction.UpdatedAt = transaction.CreatedAt;
            transaction.Product = product;

            if (ModelState.IsValid)
            {
                buyer.Balance -= Amount * product.Price;
                linq.First().seller.Balance += Amount * product.Price;
                product.Amount -= Amount;
                _context.User.Update(buyer);
                _context.User.Update(linq.First().seller);;
                _context.Product.Update(product);
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "Color", transaction.ProductId);
            return View(transaction);
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdmin([Bind("Id,FromUserID,ToUserID,Status,CreatedAt,UpdatedAt,ProductId,Amount,PriceForOneProduct")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "Color", transaction.ProductId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transaction.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "Color", transaction.ProductId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FromUserID,ToUserID,Status,CreatedAt,UpdatedAt,ProductId,Amount,PriceForOneProduct")] Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Product, "Id", "Color", transaction.ProductId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transaction
                .Include(t => t.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transaction.FindAsync(id);
            _context.Transaction.Remove(transaction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
            return _context.Transaction.Any(e => e.Id == id);
        }
    }
}
