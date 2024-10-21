using Ecommerce.DataAccess.Repositories;
using Ecommerce.Entities.Interfaces;
using Ecommerce.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
                o => o.ApplicationUserId == userId,
                Include: "ApplicationUser"
            ).Select(order => new
            {
                id = order.Id,
                orderDate = order.OrderDate,
                orderStatus = order.OrderStatus,
                totalPrice = order.TotalPrice
            });

            return Json(new { data = userOrders });
        }
    }
}
