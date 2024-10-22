using Ecommerce.Entities.Interfaces;
using Ecommerce.Entities.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Uitilites;
using X.PagedList.Extensions;

namespace Ecommerce.Areas.Customer.Controllers
{
    
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {

            var products = _unitOfWork.Product.GetAll(Include: "Category");
            return View(products);
        }
        public IActionResult AllProducts(string? SearchInp , string? category)
        {
            if (string.IsNullOrEmpty(SearchInp) && string.IsNullOrEmpty(category))
            {
                var products = _unitOfWork.Product.GetAll(Include: "Category");
                return View(products);
            }
            else if (category != null)
            {
                var bookCategor = _unitOfWork.Category.GetFirstOrDefault(c => c.Name.ToLower() == category.ToLower());
                var products = _unitOfWork.Product.GetAll(p => p.CategoryId == bookCategor.Id, Include: "Category");
                return View(products);
            }
            else if(SearchInp != null )
            {
                var products = _unitOfWork.Product.GetAll(p => p.Name.ToLower().Contains(SearchInp), Include: "Category");
                return View(products);
            }
            return View();

        }
        
        [HttpGet]
        public IActionResult Details([FromRoute]int id)
        {
            ShoppingCart obj = new ShoppingCart()
            {
                ProductId = id,
                Product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == id, Include: "Category"),
                Count = 1
            };
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;
            shoppingCart.Id = 0;

            ShoppingCart cartobj = _unitOfWork.ShoppingCart.GetFirstOrDefault
                (c => c.ApplicationUserId == shoppingCart.ApplicationUserId && c.ProductId == shoppingCart.ProductId);
            if (cartobj == null)
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Complete();
                HttpContext.Session.SetInt32(SD.SessionKey,
                _unitOfWork.ShoppingCart.GetAll(sc=>sc.ApplicationUserId == claim.Value).ToList().Count());
               
            }
            else
            {
                _unitOfWork.ShoppingCart.IncreaseCount(cartobj, shoppingCart.Count);
                _unitOfWork.Complete();
            }

            return RedirectToAction(nameof(AllProducts));
        }
    }
}
