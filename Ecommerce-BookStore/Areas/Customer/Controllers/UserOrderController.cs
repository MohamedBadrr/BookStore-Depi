using Ecommerce.DataAccess.Repositories;
using Ecommerce.Entities.Interfaces;
using Ecommerce.Entities.Models;
using Ecommerce.Entities.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using Stripe;
using Uitilites;
using Ecommerce.Migrations;

namespace Ecommerce.Areas.User.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class UserOrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserOrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult UserOrders()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetUserOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userOrders = _unitOfWork.OrderHeader.GetAll(
                o => o.ApplicationUserId == userId && o.OrderStatus != "Cancelled",
                Include: "ApplicationUser"
            ).ToList();

            return View(userOrders);
        }
        [HttpPost]
        public IActionResult CancelOrder(int id)
        {
            var orderfromdb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id);
            if (orderfromdb.PaymentStatus == "Approve")
            {
                var option = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderfromdb.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(option);

                _unitOfWork.OrderHeader.UpdateOrderStatus(orderfromdb.Id, SD.Cancelled, SD.Refund);
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateOrderStatus(orderfromdb.Id, SD.Cancelled, SD.Cancelled);
            }
            _unitOfWork.Complete();

            return RedirectToAction("GetUserOrders", "UserOrder");
        }
        [HttpPost]
        public IActionResult CompleteOrder(int id)
        {
            var orderfromdb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id);
            var items = _unitOfWork.OrderDetail.GetAll(od=>od.OrderHeaderId == id).ToList().Count() > 1 ? _unitOfWork.OrderDetail.GetAll(od => od.OrderHeaderId == id).ToList().Count() : 1; 
            var domain = "http://localhost:7121/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"https://localhost:7121/customer/cart/orderconfirmation?id={id}",
                CancelUrl = $"https://localhost:7121/customer/cart/index"
            };

            //foreach (var item in shoppingCartVM.CartList)
            //{
                var sessionlineoption = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(orderfromdb.TotalPrice * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Competing Your Order",
                        },
                    },
                    Quantity = items,
                };
                options.LineItems.Add(sessionlineoption);
            //}

            var service = new SessionService();
            Session session = service.Create(options);
            orderfromdb.SessionId = session.Id;

            _unitOfWork.Complete();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
    }
}

    



