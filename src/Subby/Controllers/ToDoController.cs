using Subby.Core;
using Subby.Core.Entities;
using Subby.Utilities.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Subby.Web.New.ApiModels;

namespace Subby.Web.New.Controllers
{
    public class ToDoController : Controller
    {
        private readonly IRepository _repository;

        public ToDoController(IRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            var items = _repository.Linq<ToDoItem>()
                            .Select(ToDoItemDTO.FromToDoItem);
            return View(items);
        }


    }
}
