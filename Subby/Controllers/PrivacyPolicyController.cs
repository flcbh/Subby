using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Subby.Controllers
{
    public class PrivacyPolicyController : Controller
    {
        // GET: PrivacyPolicyController
        public ActionResult Index()
        {
            return View();
        }
    }
}
