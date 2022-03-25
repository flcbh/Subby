using Microsoft.AspNetCore.Mvc;

namespace Subby.Web.New.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiController : Controller
    {
    }
}
