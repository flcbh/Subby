using System;
using System.Linq;
using Subby.Utilities.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Subby.Core.Entities;

namespace Subby.Blazor.Maui.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly IRepository _repository;

        public PaymentsController(IRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Complete()
        {
            return View();
        }
    }
}