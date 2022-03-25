using Microsoft.AspNetCore.Mvc;

namespace Subby.Blazor.Maui.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiController : Controller
    {
    }
}
