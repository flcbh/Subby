using System;
using System.Collections.Generic;
using Subby.Core.Entities;
using Subby.Utilities.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stripe;
using Subby.Core.Events;
using Subby.Core.Extensions;
using Subby.Utilities.DomainEvents;
using Subby.ApiModels;
using Subby.Models.PaymentViewModels;

namespace Subby.Api
{
    public class StripeController : BaseApiController
    {
        private readonly IRepository _repository;
        private readonly IDomainEvents _events;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _environment;
        public StripeController(IRepository repository, IDomainEvents events, ILogger<StripeController> logger, IWebHostEnvironment environment)
        {
            _repository = repository;
            _events = events;
            _logger = logger;
            _environment = environment;
        }

        [HttpGet("public-key")]
        public IActionResult PublicKey()
        {
            var premiumPrice = _repository.Linq<Configuration>().GetOrDefault<decimal>("PREMIUM_PRICE", 4.99m);
            var annualPrice = _repository.Linq<Configuration>().GetOrDefault<decimal>("ANNUAL_PRICE", 49m);
            var data = new PaymentPlanViewModel
            {
                PublicKey = _environment.EnvironmentName != "Development" ? "pk_live_2JilGHOXtMiInLbRqeE3dpFd00Kg6ARxW7" : "pk_test_EPmojs2YlEIiROTbKd7pivQH00WYZlhLxO",
                Plans = new List<PaymentPlan>
                {
                    new PaymentPlan
                    {
                        Key = "Monthly",
                        Amount = premiumPrice
                    },
                    new PaymentPlan
                    {
                        Key = "Annually",
                        Amount = annualPrice
                    }
                }
            };

            return new JsonResult(data);
        }

        [HttpPost("capture")]
        public IActionResult Capture(CapturePaymentViewModel model)
        {
            StripeConfiguration.ApiKey = _environment.EnvironmentName != "Development" ? "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" : "sk_test_b7iwHF3VuKAfchtFGteRcv15009K5g6bk2";

            var premiumPrice = _repository.Linq<Configuration>().GetOrDefault<decimal>("PREMIUM_PRICE", 4.99m);
            var expiryDate = DateTime.Now.AddMonths(1);
            switch (model.Plan)
            {
                case "Annually":
                    premiumPrice = _repository.Linq<Configuration>().GetOrDefault<decimal>("ANNUAL_PRICE", 49m);
                    expiryDate = DateTime.Now.AddYears(1);
                    break;
            }

            var paymentIntentService = new PaymentIntentService();
            var intent = paymentIntentService.Get(model.Token);

            if (intent.Status == "succeeded")
            {
                var user = _repository.Linq<User>().FirstOrDefault(x => x.Email == User.Identity.Name);
                var transaction = new Transaction
                {
                    Amount = premiumPrice,
                    Reference = model.Token,
                    Currency = "gbp",
                    Status = Transaction.Statuses.Succeeded,
                    Response = JsonConvert.SerializeObject(intent)
                };

                _repository.Add(transaction);

                var subscription = new Subby.Core.Entities.Subscription
                {
                    Amount = premiumPrice,
                    CreatedAt = DateTime.Now,
                    ExpiryDate = expiryDate,
                    Transaction = transaction,
                    User = user,
                    Plan = model.Plan
                };

                _repository.Add(subscription);

                var data = new
                {
                    id = subscription.Id,
                    amount = subscription.Amount
                };

                return Ok(data);
            }

            return BadRequest(intent.Status);
        }



        // [HttpPost("capture")]
        // public IActionResult CaptureOld(CapturePaymentViewModel model)
        // {
        //     var user = _repository.Linq<User>().FirstOrDefault(x => x.Email == User.Identity.Name);
        //     StripeConfiguration.ApiKey = _environment.EnvironmentName != "Development" ? "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" : "sk_test_b7iwHF3VuKAfchtFGteRcv15009K5g6bk2";
        //     var premiumPrice = _repository.Linq<Configuration>().GetOrDefault<decimal>("PREMIUM_PRICE", 4.99m);
        //     var expiryDate = DateTime.Now.AddMonths(1);
        //     switch (model.Plan)
        //     {
        //         case "Annually":
        //             premiumPrice = _repository.Linq<Configuration>().GetOrDefault<decimal>("ANNUAL_PRICE", 49m);
        //             expiryDate = DateTime.Now.AddYears(1);
        //             break;
        //     }
        //     
        //     var options = new ChargeCreateOptions
        //     {
        //         Amount = ConvertAmount(premiumPrice),
        //         Currency = "gbp",
        //         Description = $@"Premium subscription payment for {user?.FirstName} {user?.LastName}",
        //         Source = model.Token,
        //         ReceiptEmail = user?.Email
        //     };
        //
        //     try
        //     {
        //         var service = new ChargeService();
        //         var charge = service.Create(options);
        //
        //         if (charge.Status == "succeeded")
        //         {
        //             var transaction = new Transaction
        //             {
        //                 Amount = premiumPrice,
        //                 Reference = model.Token,
        //                 Currency = "gbp",
        //                 Status = Transaction.Statuses.Succeeded,
        //                 Response = JsonConvert.SerializeObject(charge)
        //             };
        //
        //             _repository.Add(transaction);
        //     
        //             var subscription = new Subby.Core.Entities.Subscription
        //             {
        //                 Amount = premiumPrice,
        //                 CreatedAt = DateTime.Now,
        //                 ExpiryDate = expiryDate,
        //                 Transaction = transaction,
        //                 User = user,
        //                 Plan = model.Plan
        //             };
        //     
        //             _repository.Add(subscription);
        //
        //             var data = new
        //             {
        //                 id = subscription.Id,
        //                 amount = subscription.Amount
        //             };
        //     
        //             return Ok(data);
        //         }
        //         
        //         return BadRequest(charge.FailureMessage);
        //     }
        //     catch (StripeException e)
        //     {
        //         var errorMessage = string.Empty;
        //         switch (e.StripeError.Error)
        //         {
        //             case "card_error":
        //                 _logger.LogCritical(LogPaymentError("card_error", user?.Email, e), e);
        //                 errorMessage = e.StripeError.Message;
        //                 break;
        //             case "api_connection_error":
        //                 _logger.LogCritical(LogPaymentError("api_connection_error", user?.Email, e), e);
        //                 errorMessage = e.StripeError.Message;
        //                 break;
        //             case "api_error":
        //                 _logger.LogCritical(LogPaymentError("api_error", user?.Email, e), e);
        //                 errorMessage = e.StripeError.Message;
        //                 break;
        //             case "authentication_error":
        //                 _logger.LogCritical(LogPaymentError("authentication_error", user?.Email, e), e);
        //                 errorMessage = e.StripeError.Message;
        //                 break;
        //             case "invalid_request_error":
        //                 _logger.LogInformation(LogPaymentError("invalid_request_error", user?.Email, e), e);
        //                 errorMessage = e.StripeError.Message;
        //                 break;
        //             case "rate_limit_error":
        //                 _logger.LogInformation(LogPaymentError("rate_limit_error", user?.Email, e), e);
        //                 errorMessage = e.StripeError.Message;
        //                 break;
        //             case "validation_error":
        //                 _logger.LogInformation(LogPaymentError("validation_error", user?.Email, e), e);
        //                 errorMessage = e.StripeError.Message;
        //                 break;
        //             default:
        //                 // Unknown Error Type
        //                 errorMessage = e.StripeError.Message;
        //                 break;
        //         }
        //
        //         return BadRequest(errorMessage);
        //     }
        // }

        // [HttpPost("payment-intents")]
        // public IActionResult Payment([FromBody] CreatePaymentViewModel model)
        // {
        //     var user = _repository.Linq<User>().FirstOrDefault(x => x.Email == User.Identity.Name);
        //     var premiumPrice = _repository.Linq<Configuration>().GetOrDefault<decimal>("PREMIUM_PRICE", 4.99m);
        //     var amount = premiumPrice;
        //     var payment = new
        //     {
        //         Amount = ConvertAmount(amount),
        //         Currency = "gbp",
        //         ReceiptEmail = user?.Email
        //     };
        //
        //     return new JsonResult(payment);
        // }

        [HttpPost("payment-intents")]
        public IActionResult PaymentIntent(CapturePaymentViewModel model)
        {
            var user = _repository.Linq<User>().FirstOrDefault(x => x.Email == User.Identity.Name);
            StripeConfiguration.ApiKey = _environment.EnvironmentName != "Development" ? "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" : "sk_test_b7iwHF3VuKAfchtFGteRcv15009K5g6bk2";
            var premiumPrice = _repository.Linq<Configuration>().GetOrDefault<decimal>("PREMIUM_PRICE", 4.99m);

            switch (model.Plan)
            {
                case "Annually":
                    premiumPrice = _repository.Linq<Configuration>().GetOrDefault<decimal>("ANNUAL_PRICE", 49m);
                    break;
            }

            var paymentIntents = new PaymentIntentService();
            var paymentIntent = paymentIntents.Create(new PaymentIntentCreateOptions
            {
                Amount = ConvertAmount(premiumPrice),
                Currency = "gbp",
                Description = $@"Premium subscription payment for {user?.FirstName} {user?.LastName}",
                ReceiptEmail = user?.Email
            });

            return Json(new { clientSecret = paymentIntent.ClientSecret });
        }


        private int ConvertAmount(decimal amount)
        {
            return (int)(amount * 100);
        }

        private static string LogPaymentError(string errorType, string email, StripeException e)
        {
            return string.Format("Payment {0} {1}:{2} , email: {3}", errorType, e.StripeError.Code, e.StripeError.Message, email);
        }

    }
}
