using Subby.Core;
using Subby.Core.Entities;
using Subby.Utilities.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Subby.Web.ApiModels;

namespace Subby.Web.Controllers
{
    public class ProductsController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IRepository _repository;

        public ProductsController(IRepository repository)
        {
            _repository = repository;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/product/{sku}")]
        public IActionResult Details(string sku)
        {
            return View();
        }
    }
}
