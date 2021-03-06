using Recycle.App_Custom.Helpers.Objects;
using Recycle.App_Custom.Helpers.ViewModelHelpers;
using Recycle.App_Custom.Services.ClientCurrency;
using Recycle.Areas.Admin.ViewModels.Home;
using Recycle.Data;
using Recycle.Models;
using Recycle.Models.Helpers;
using Recycle.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Recycle;

namespace Recycle.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly RecycleContext _dbContext;
        private readonly OrdersService _orders;
        private readonly ClientCurrencyService _clientCurrency;

        public HomeController(RecycleContext dbContext, OrdersService orders, ClientCurrencyService clientCurrency)
        {
            _dbContext = dbContext;
            _orders = orders;
            _clientCurrency = clientCurrency;
        }

        public async Task<IActionResult> Index(IndexVM model)
        {
            // Gets the number of orders awaiting:
            ViewData["Results_OrdersAwaiting"] = await _dbContext.Orders.Where(o => o.Status == OrderStatus.Packing).CountAsync();

            // Gets the number of pending contacts:
            ViewData["Results_PendingContacts"] = await _dbContext.Contacts.Where(c => c.Status == ContactStatus.Pending).CountAsync();

            DateTime fromDate = this.GetDateTimeStartOfPeriod(model.Period);

            // Gets the number of new customers (in this period):
            ViewData["Results_NewCustomers"] = await _dbContext.Users.Where(u => u.DateRegistered >= fromDate && u.Role == UserRole.Customer).CountAsync();


            // Gets all the orders related to the specified period:
            List<Order> orders = await _orders.GetAllFromDateTime(fromDate);

            // If period is liftime - overrides the fromDate to be the first record:
            if (orders.Count > 0 && model.Period == Periods.Lifetime)
                fromDate = orders.Last().DateCreated;


            // Get total orders:
            ViewData["All_Orders"] = orders.Count;

            // Gets the total earnings and earnings time series chart:
            double totalEarnings = 0;
            foreach (Order order in orders)
            {
                double orderTotal = order.GetTotalPrice();
                totalEarnings += orderTotal;
            }
            ViewData["Results_TotalEarnings"] = _clientCurrency.GetPrice(totalEarnings);
            ViewData["Results_AvgCustomerSpendingTS"] = (totalEarnings/orders.Count);

            return View(model);
        }

        #region Private Methods

        /// <summary>
        /// Gets the date and time of the period start.
        /// </summary>
        /// <param name="period">The period to get the start date time.</param>
        /// <returns>Returns the date and time the period starts.</returns>
        private DateTime GetDateTimeStartOfPeriod(Periods period)
        {
            if (period == Periods.Today)
            {
                return DateTime.Today;
            }
            else if (period == Periods.ThisMonth)
            {
                return new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }
            else if (period == Periods.ThisYear)
            {
                return new DateTime(DateTime.Now.Year, 1, 1);
            }
            else if (period == Periods.Lifetime)
            {
                return DateTime.MinValue;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(period), $"{nameof(period)} is not defined.");
            }
        }

        #endregion

    }
}
