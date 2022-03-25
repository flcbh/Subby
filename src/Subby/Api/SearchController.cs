using Subby.Core.Entities;
using Subby.Utilities.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Subby.Core.Events;
using Subby.Utilities.DomainEvents;
using Subby.Web.New.ApiModels;

namespace Subby.Web.New.Api
{
    public class SearchController : BaseApiController
    {
        private readonly IRepository _repository;

        public SearchController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult Search(string phrase)
        {
            var item = _repository.Linq<Job>().Where(x => x.Title.Contains(phrase)
                                                          || x.Postcode.Contains(phrase)
                                                          || x.Location.Contains(phrase)).ToList();
            var data = item.Select(x => new
            {
                name = x.Title,
                url = x.Slug,
                location = x.Location
            }).ToList();

            return Json(data);
        }
    }
}
