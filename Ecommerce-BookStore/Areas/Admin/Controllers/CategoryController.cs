using Ecommerce.DataAccess.Data;
using Ecommerce.DataAccess.Repositories;
using Ecommerce.Entities.Interfaces;
using Ecommerce.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //Index
        public IActionResult Index()
        {
            var categories = _unitOfWork.Category.GetAll();
            return View(categories);
        }

        // Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                //_dbContext.Categories.Add(category);
                //_dbContext.SaveChanges();
                _unitOfWork.Category.Add(category);
                _unitOfWork.Complete();
                TempData["Create"] = "Category Has Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        //details 
        public IActionResult Details([FromRoute] int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }
            //var category = _dbContext.Categories.Find(id);
            var category = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == id);
            if (category is null)
            {
                return NotFound();
            }
            return View(category);
        }
        // Update 
        public IActionResult Update([FromRoute] int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }
            var category = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == id);
            if (category is null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(category);
                _unitOfWork.Complete();
                TempData["Update"] = "Category Has Updated Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        //Delete
        public IActionResult Delete([FromRoute] int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }
            var category = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == id);
            if (category is null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Category category)
        {
            try
            {
                _unitOfWork.Category.Remove(category);
                _unitOfWork.Complete();
                TempData["Delete"] = "Category Has Deleted Successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(category);
            }
        }
    }
}
