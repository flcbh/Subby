using Microsoft.AspNetCore.Mvc;

namespace Subby.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiController : Controller
    {
    }
}
