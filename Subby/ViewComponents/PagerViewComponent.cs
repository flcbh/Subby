using System.Threading.Tasks;
using Subby.Utilities.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace Subby.ViewComponents
{
    public class PagerViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(LastContent.Utilities.Pagination.PagedResultBase result)
        {
            return Task.FromResult((IViewComponentResult)View("Default", result));
        }
    }
}