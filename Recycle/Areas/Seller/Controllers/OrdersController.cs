using Recycle.App_Custom.Helpers.ViewModelHelpers;
using Recycle.Areas.Seller.ViewModels.Orders;
using Recycle.Models;
using Recycle.Models.Helpers;
using Recycle.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recycle.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize(Roles = "Seller")]
    public class OrdersController : Controller
    {
        private readonly OrdersService _orders;
        private readonly UserIdentityService _userIdentity;

        public OrdersController(OrdersService orders, UserIdentityService userIdentity)
        {
            _orders = orders;
            _userIdentity = userIdentity;
        }

        // GET: /Seller/Orders
        public async Task<IActionResult> Index(IndexVM model)
        {
            int? userId = _userIdentity.GetCurrentId();
            if (userId == null)
                return NotFound();

            List<Order> orders = await _orders.Search(model.Status, model.DateCreated, model.OrderId);

            #region Pagination...

            Pagination pagination = new Pagination(orders.Count, model.PageSize, model.Page);
            if (pagination.HasPagination())
            {
                if (pagination.PageSize.HasValue)
                {
                    orders = orders
                        .Skip(pagination.GetRecordsSkipped())
                        .Take(pagination.PageSize.Value)
                        .ToList();
                }
            }
            ViewData["Pagination"] = pagination;

            #endregion

            ViewData["OrdersModel"] = orders;
            return View(model);
        }

        // GET: /Seller/Orders/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            Order order = await _orders.GetByIdAsync(id);
            if (order == null)
                return NotFound();
            else
                return View(order);
        }

        // GET: /Seller/Orders/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            Order order = await _orders.GetByIdAsync(id);
            if (order == null)
                return NotFound();
            else
                return View(order);
        }

        // POST: /Seller/Orders/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete_POST(int id)
        {
            await _orders.DeleteOrder(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: /Seller/Orders/IndexUpdateStatus/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexUpdateStatus(int id, OrderStatus status)
        {
            await _orders.UpdateStatus(id, status);
            return RedirectToAction(nameof(Index));
        }

        // POST: /Seller/Orders/DetailsUpdateStatus/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsUpdateStatus(int id, OrderStatus status)
        {
            await _orders.UpdateStatus(id, status);
            return RedirectToAction(nameof(Details), new { id = id });
        }

    }
}
