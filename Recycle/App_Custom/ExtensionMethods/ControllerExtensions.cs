using Microsoft.AspNetCore.Mvc;

namespace Recycle
{
    /// <summary>
    /// Represents extension methods for <see cref="Controller"/>.
    /// </summary>
    public static class ControllerExtensions
    {

        /// <summary>
        /// Redirects to the home page.
        /// </summary>
        public static RedirectToActionResult RedirectToHomePage(this Controller controller)
        {
            return controller.RedirectToAction(nameof(Recycle.Controllers.HomeController.Index), "Home", new { area = "" });
        }

    }
}
