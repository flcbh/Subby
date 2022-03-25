
using System;
using Subby.Core.Entities;
using Subby.Utilities.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Subby.Core.Events;
using Subby.Utilities.DomainEvents;
using Subby.Web.ApiModels;

namespace Subby.Web.Api
{
    public class ImagesController : BaseApiController
    {
        private readonly IRepository _repository;
        private readonly IDomainEvents _events;
        
        public ImagesController(IRepository repository, IDomainEvents events)
        {
            _repository = repository;
            _events = events;
        }


        [HttpPost("[action]/{id}")]
        public IActionResult Delete(int id)
        {
            var media = _repository.Linq<Media>().FirstOrDefault(x => x.Id == id);
            
            if (media != null)
            {
                _repository.Delete(media);
                _repository.Save();
            }
            
            return Ok();
        }
    }
}
