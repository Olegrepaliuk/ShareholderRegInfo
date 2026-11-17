using Microsoft.AspNetCore.Mvc;
using ShareholderRegInfo.Portal.Models;
using ShareholderRegInfo.Portal.Services;
using System.Diagnostics;

namespace ShareholderRegInfo.Portal.Controllers
{
    public class HomeController(IRegisterService registerService) : Controller
    {
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var result = await registerService.LoadAsync(cancellationToken);

            var model = new RegisterViewModel
            {
                Form = result.Form,
                Warnings = result.Warnings,
                ErrorMessage = result.ErrorMessage,
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
