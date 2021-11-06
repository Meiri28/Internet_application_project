using Recycle.App_Custom.ActionFilters;
using Recycle.App_Custom.Services.ClientCurrency;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Recycle.Controllers
{
    public class PreferencesController : Controller
    {
        private readonly ClientCurrencyService _clientCurrency;

        public PreferencesController( ClientCurrencyService clientCurrency)
        {
            _clientCurrency = clientCurrency;
        }

        #region AJAX Actions

        [Route("/Preferences/UpdateClientCurrency")]
        [AjaxOnly]
        [HttpPost]
        public async Task<IActionResult> UpdateClientCurrency([FromBody] string currencyCode)
        {
            if (string.IsNullOrEmpty(currencyCode))
                return Json(false);

            await _clientCurrency.SetAsync(currencyCode);
            return Json(true);
        }

        #endregion

    }
}
