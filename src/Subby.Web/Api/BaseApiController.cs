using Microsoft.AspNetCore.Mvc;

namespace Subby.Web.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiController : Microsoft.AspNetCore.Mvc.Controller
    {
    }
}
