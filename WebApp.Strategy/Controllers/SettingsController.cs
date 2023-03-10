using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using WebApp.Strategy.Models;

namespace WebApp.Strategy.Controllers
{
    public class SettingsController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            Settings settings = new();
            if (User.Claims.Where(x => x.Type == Settings.claimDatabaseType).FirstOrDefault() != null)
            {
                settings.databaseType = (EDatabaseType)int.Parse(User.Claims.First(x => x.Type == Settings.claimDatabaseType).Value);
            }
            else
            {
                settings.databaseType = settings.getDefaultDatabaseType;
            }
            return View(settings);
        }
    }
}
