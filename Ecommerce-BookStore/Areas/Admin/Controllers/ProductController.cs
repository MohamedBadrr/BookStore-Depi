using Ecommerce.DataAccess.Data;
using Ecommerce.DataAccess.Repositories;
using Ecommerce.Entities.Interfaces;
using Ecommerce.Entities.Models;
using Ecommerce.Entities.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork , IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        //Index
        public IActionResult Index()
        {
            var products = _unitOfWork.Product.GetAll();
            return View(products);
        }
        public IActionResult GetData()
        {
            var products = _unitOfWork.Product.GetAll(Include: "Category");
            return Json(new {data = products });
        }

        // Create
        public IActionResult Create()
        {
            ProductVM prouctVM = new ProductVM()
            {
                product = new Product(),
                categoryList = _unitOfWork.Category.GetAll().Select(C => new SelectListItem{
                     Text = C.Name, Value = C.Id.ToString(),
                }),
            };
            return View(prouctVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductVM productVM, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                string RootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var Upload = Path.Combine(RootPath, @"Images\Products");
                    var ext = Path.GetExtension(file.FileName);

                    using (var filestream = new FileStream(Path.Combine(Upload, fileName + ext), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
                    productVM.product.Img = @"Images\Products\" + fileName + ext;
                }

                _unitOfWork.Product.Add(productVM.product);
                _unitOfWork.Complete();
                TempData["Create"] = "Product Has Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(productVM.product);
        }

        //details 
        public IActionResult Details([FromRoute] int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }
            var product = _unitOfWork.Product.GetFirstOrDefault(c => c.Id == id, Include: "Category");
            product.Category = _unitOfWork.Category.GetFirstOrDefault(c=> c.Id == product.CategoryId);
            if (product is null)
            {
                return NotFound();
            }
            return View(product);
        }
        // Update 
        public IActionResult Update([FromRoute] int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }
            ProductVM productVM = new ProductVM()
            {
                product = _unitOfWork.Product.GetFirstOrDefault(c => c.Id == id),
                categoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() })
            };
            if (productVM is null)
            {
                return NotFound();
            }
            return View(productVM); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(ProductVM productVM , IFormFile? file )
        {
            if (ModelState.IsValid)
            {
                string RootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var Upload = Path.Combine(RootPath, @"Images\Products");
                    var ext = Path.GetExtension(file.FileName);

                    if (productVM.product.Img  != null)
                    {
                        var oldImg = Path.Combine(RootPath , productVM.product.Img.TrimStart('\\'));
                        if(System.IO.File.Exists(oldImg))
                        {
                            System.IO.File.Delete(oldImg);
                        }
                    }

                    using (var filestream = new FileStream(Path.Combine(Upload, fileName + ext), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
                    productVM.product.Img = @"Images\Products\" + fileName + ext;
                }

                _unitOfWork.Product.Update(productVM.product);
                _unitOfWork.Complete();
                TempData["Update"] = "Product Has Updated Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(productVM.product);
        }
        //Delete
        [HttpDelete]
        public IActionResult Delete([FromRoute] int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }
            var product = _unitOfWork.Product.GetFirstOrDefault((c => c.Id == id));
            if (product is null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }
            _unitOfWork.Product.Remove(product);
            var oldImg = Path.Combine(_webHostEnvironment.WebRootPath, product.Img.TrimStart('\\'));
            if (System.IO.File.Exists(oldImg))
            {
                System.IO.File.Delete(oldImg);
            }
            _unitOfWork.Complete();
            return Json(new { success = true, message = "File has Been Deleted" });
        }

    }
}
