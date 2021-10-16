using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Recycle.Data;
using Recycle.Models;

namespace Recycle.Controllers
{
    [Authorize(Roles ="Admin")]
    public class UsersController : Controller
    {
        private readonly RecycleContext _context;

        public UsersController(RecycleContext context)
        {
            _context = context;
        }

        // GET: Users
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.User.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        [AllowAnonymous]
        public IActionResult Create()
        {
            ViewBag.UserGender = new SelectList(_context.Set<UserGender>(), nameof(UserGender.Id), nameof(UserGender.Title));
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Email,Password,BDay")] User user, int Gender)
        {
            if(_context.User.Where(u => u.Email == user.Email).Count() > 0)
            {
                ViewData["error_code"] = "This Email is already registerd";
                return View(user);
            }
            if(user.BDay > DateTime.Now)
            {
                ViewData["error_code"] = "users must be born to register this site";
                return View(user);
            }
            if (ModelState.IsValid)
            {
                user.Gender = _context.UserGender.First(x => x.Id == Gender);
                user.UpdatedAt = user.CreatedAt;
                user.IsActive = true;
                user.Password = hash_passwrd(user.Password);
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(LogIn));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,Password,Gender,BDay,Balance")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.UpdatedAt = DateTime.Now;
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Users/LogIn\
        [AllowAnonymous]
        public async Task<IActionResult> LogIn()
        {

            return View();
        }

        // POST: Users/LogIn
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> LogIn(string Email,string Password)
        {
            string hashed_password = hash_passwrd(Password);
            var users = from user in _context.User
                       where user.IsActive && user.Email == Email && user.Password == hashed_password
                        select user;
            if(users.Count() == 0)
            {
                ViewData["error_code"] = "username or passward is incorrect"; 
                return View();

            }
            var account = users.First();

            var claims = new List<Claim> {
                                            new Claim(ClaimTypes.Email, account.Email),
                                            new Claim(ClaimTypes.Name, account.FirstName + " " + account.LastName),
            };
            if (_context.Store.Any(S => S.UserId == account.Id))
            {
                claims.Add(new Claim(ClaimTypes.Role, "Seller"));
            } else
            {
                claims.Add(new Claim(ClaimTypes.Role, "Buyer"));
            }
            if (_context.Admin.Any(S => S.UserId == account.Id))
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10)
            };
             HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                           new ClaimsPrincipal(claimsIdentity),authProperties);

            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(nameof(Index),"Home");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConnectAs(int? id)
        {
            if(id == null)
                return View(await _context.User.ToListAsync());

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            User account = _context.User.First(u => u.Id == id);
            if (account == null)
            {
                ViewData["error_code"] = "User not found";
                return View();
            }

            var claims = new List<Claim> {
                                            new Claim(ClaimTypes.Email, account.Email),
                                            new Claim(ClaimTypes.Name, account.FirstName + " " + account.LastName),
            };
            if (_context.Store.Any(S => S.UserId == account.Id))
            {
                claims.Add(new Claim(ClaimTypes.Role, "Seller"));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, "Buyer"));
            }
            if (_context.Admin.Any(S => S.UserId == account.Id))
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10)
            };
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                          new ClaimsPrincipal(claimsIdentity), authProperties);

            return RedirectToAction(nameof(Index), "Home");
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }

        private string hash_passwrd(string origin_password)
        {
            byte[] salt = new byte[128/8];
            salt = Encoding.ASCII.GetBytes("/-_#*+!?()=:.@");
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: origin_password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));
        }
    }
}
