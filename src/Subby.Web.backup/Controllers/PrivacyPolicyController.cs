using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Subby.Web.Controllers
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
