using Subby.Core;
using Subby.Utilities.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Subby.Web.Pages.ToDoRazorPage
{
    public class PopulateModel : PageModel
    {
        private readonly IRepository _repository;

        public PopulateModel(IRepository repository)
        {
            _repository = repository;
        }

        public int RecordsAdded { get; set; }
    }
}
