using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Subby.Web.Controllers
{
    public class PrivacyPolicyController : Microsoft.AspNetCore.Mvc.Controller
    {
        // GET: PrivacyPolicyController
        public Microsoft.AspNetCore.Mvc.ActionResult Index()
        {
            return View();
        }
    }
}
