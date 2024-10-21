using Ecommerce.DataAccess.Repositories;
using Ecommerce.Entities.Interfaces;
using Ecommerce.Entities.Models;
using Ecommerce.Entities.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Uitilites;

namespace Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var orders = _unitOfWork.OrderHeader.GetAll(Include: "ApplicationUser");
            return View(orders);
        }
        [HttpGet]
        public IActionResult GetData()
        {
            IEnumerable<OrderHeader> orderHeaders;
            orderHeaders = _unitOfWork.OrderHeader.GetAll(Include: "ApplicationUser");
            return Json(new { data = orderHeaders });
        }
        public IActionResult Details(int id)
        {
            OrderVM orderVM = new OrderVM()
            {
                orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(oh => oh.Id == id, Include: "ApplicationUser"),
                orderDetails = _unitOfWork.OrderDetail.GetAll(od => od.OrderHeaderId == id, Include: "Product")
            };
            var vm = orderVM;
            return View(orderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderDetails()
        {
            var orderfromdb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.orderHeader.Id);
            orderfromdb.Name = OrderVM.orderHeader.Name;
            orderfromdb.Phone = OrderVM.orderHeader.Phone;
            orderfromdb.Address = OrderVM.orderHeader.Address;
            orderfromdb.City = OrderVM.orderHeader.City;

            if (OrderVM.orderHeader.Carrier != null)
            {
                orderfromdb.Carrier = OrderVM.orderHeader.Carrier;
            }

            if (OrderVM.orderHeader.TrackingNumber != null)
            {
                orderfromdb.TrackingNumber = OrderVM.orderHeader.TrackingNumber;
            }

            _unitOfWork.OrderHeader.Update(orderfromdb);
            _unitOfWork.Complete();
            TempData["Update"] = "Item has Updated Successfully";
            return RedirectToAction("Details", "Order", new { id = orderfromdb.Id });
        }
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult StartProccess()
		{
			_unitOfWork.OrderHeader.UpdateOrderStatus(OrderVM.orderHeader.Id, SD.Proccessing, null);
			_unitOfWork.Complete();

			TempData["Update"] = "Order Status has Updated Successfully";
			return RedirectToAction("Details", "Order", new { id = OrderVM.orderHeader.Id });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult StartShip()
		{
			var orderfromdb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.orderHeader.Id);
			orderfromdb.TrackingNumber = OrderVM.orderHeader.TrackingNumber;
			orderfromdb.Carrier = OrderVM.orderHeader.Carrier;
			orderfromdb.OrderStatus = SD.Shipped;
			orderfromdb.ShippingDate = DateTime.Now;

			_unitOfWork.OrderHeader.Update(orderfromdb);
			_unitOfWork.Complete();

			TempData["Update"] = "Order has Shipped Successfully";
			return RedirectToAction("Details", "Order", new { id = OrderVM.orderHeader.Id });
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult CancelOrder()
		{
			var orderfromdb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.orderHeader.Id);
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

			TempData["Update"] = "Order has Cancelled Successfully";
			return RedirectToAction("Index", "Order");
		}
	}
}
