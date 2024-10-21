using Ecommerce.Entities.Interfaces;
using Ecommerce.Entities.Models;
using Ecommerce.Entities.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.BillingPortal;
using Stripe.Checkout;
using Stripe.FinancialConnections;
using System.Security.Claims;
using Uitilites;
using Session = Stripe.Checkout.Session;
using SessionCreateOptions = Stripe.Checkout.SessionCreateOptions;
using SessionService = Stripe.Checkout.SessionService;

namespace Ecommerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartVM shoppingCartVm { get; set; }
        public int TotalCarts { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCartVm = new ShoppingCartVM()
            {
                CartList = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value, Include: "Product")
            };

            foreach (var item in shoppingCartVm.CartList)
            {
                shoppingCartVm.TotalCarts += (item.Count * item.Product.Price);
            }
            return View(shoppingCartVm);
        }
        public IActionResult Minus(int cartid)
        {
            var shoppingcart = _unitOfWork.ShoppingCart.GetFirstOrDefault(sc => sc.Id == cartid);
            if (shoppingcart.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(shoppingcart);
                var count = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == shoppingcart.ApplicationUserId).ToList().Count() - 1;
                HttpContext.Session.SetInt32(SD.SessionKey, count);
                _unitOfWork.Complete();
                return RedirectToAction("Index","Home");
            }
            else
            {
                _unitOfWork.ShoppingCart.DecreaseCount(shoppingcart, 1);
                _unitOfWork.Complete();
                return RedirectToAction("Index");
            }

        }
        public IActionResult Plus(int cartid)
        {
            var shoppingcart = _unitOfWork.ShoppingCart.GetFirstOrDefault(sc => sc.Id == cartid);
            _unitOfWork.ShoppingCart.IncreaseCount(shoppingcart, 1);
            _unitOfWork.Complete();
            return RedirectToAction("Index");
        }
        public IActionResult Remove(int cartid)
        {
            var shoppingcart = _unitOfWork.ShoppingCart.GetFirstOrDefault(sc => sc.Id == cartid);
            _unitOfWork.ShoppingCart.Remove(shoppingcart);
            _unitOfWork.Complete();
            var count = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == shoppingcart.ApplicationUserId).ToList().Count();
            HttpContext.Session.SetInt32(SD.SessionKey, count);
            int numberofunits = _unitOfWork.ShoppingCart.CountItems();
            if (numberofunits > 1)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index");
        }
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
             shoppingCartVm = new ShoppingCartVM()
            {
                CartList = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value, Include: "Product"),
                OrderHeader = new()
            };
            shoppingCartVm.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(ap => ap.Id == claim.Value);
            shoppingCartVm.OrderHeader.Name = shoppingCartVm.OrderHeader.ApplicationUser.Name;
            shoppingCartVm.OrderHeader.Address = shoppingCartVm.OrderHeader.ApplicationUser.Adress;
            shoppingCartVm.OrderHeader.City = shoppingCartVm.OrderHeader.ApplicationUser.City;
            shoppingCartVm.OrderHeader.Phone = shoppingCartVm.OrderHeader.ApplicationUser.PhoneNumber;
            foreach (var item in shoppingCartVm.CartList)
            {
                shoppingCartVm.TotalCarts += (item.Count * item.Product.Price);
            }
                return View(shoppingCartVm);
        }
          

        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public IActionResult Summary(ShoppingCartVM shoppingCartVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            shoppingCartVM.CartList = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value, Include: "Product");
            
            shoppingCartVM.OrderHeader.OrderStatus = "Pinding";
            shoppingCartVM.OrderHeader.PaymentStatus = "Pinding";
            shoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            shoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
            foreach (var item in shoppingCartVM.CartList)
            {
                shoppingCartVM.TotalCarts += (item.Count * item.Product.Price);
            }
            shoppingCartVM.OrderHeader.TotalPrice = shoppingCartVM.TotalCarts;
			_unitOfWork.OrderHeader.Add(shoppingCartVM.OrderHeader);
            _unitOfWork.Complete();


            //useStrip
            var domain = "http://localhost:7121/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"https://localhost:7121/customer/cart/orderconfirmation?id={shoppingCartVM.OrderHeader.Id}",
                CancelUrl = $"https://localhost:7121/customer/cart/index"
            };
            
            foreach (var item in shoppingCartVM.CartList) {
                var sessionlineoption = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price*100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                        },
                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionlineoption);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            shoppingCartVM.OrderHeader.SessionId = session.Id;

            _unitOfWork.Complete();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public IActionResult OrderConfirmation(int id ) { 
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(oh=>oh.Id == id);
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeader.UpdateOrderStatus(id, SD.Approve, SD.Approve);
                orderHeader.PaymentIntentId = session.PaymentIntentId;
                _unitOfWork.Complete();
            }
            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u=>u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Complete();
            return View(id);
        }

    }
}
