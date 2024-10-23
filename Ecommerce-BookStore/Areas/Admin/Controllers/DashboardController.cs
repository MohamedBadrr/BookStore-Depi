using Ecommerce.DataAccess.Repositories;
using Ecommerce.Entities.Interfaces;
using Ecommerce.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Uitilites;

namespace Ecommerce.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            ViewBag.Orders = _unitOfWork.OrderHeader.GetAll().Count();
            ViewBag.Categories = _unitOfWork.Category.GetAll().Count();
            ViewBag.Users = _unitOfWork.ApplicationUser.GetAll().Count();
            ViewBag.Products = _unitOfWork.Product.GetAll().Count();

            return View();
        }
    }
}
