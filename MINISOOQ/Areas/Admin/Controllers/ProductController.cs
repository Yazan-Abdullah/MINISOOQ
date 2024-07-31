using MINISOOQ.DataAccess;
using MINISOOQ.DataAccess.Repository.IRepository;
using MINISOOQ.Models;
using MINISOOQ.Models.ViewModels;
using MINISOOQ.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MINISOOQ.Controllers;
[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _hostEnvironment;


    public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _hostEnvironment = hostEnvironment;
    }
    public IActionResult Index()
    {
        return View();
    }
    //7/15/2024

    //public IActionResult Upsert(int? id)
    //{
    //    ProductVM productVM = new()
    //    {
    //        Product = new(),
    //        CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
    //        {
    //            Text = i.Name,
    //            Value = i.Id.ToString()
    //        }),
    //    };
    //    if (id == null || id == 0)
    //    {
    //        return View(productVM);
    //    }
    //    else
    //    {
    //        productVM.Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
    //        return View(productVM);

    //    }
    //}
    public IActionResult Upsert(int? id)
    {
        ProductVM productVM = new()
        {
            Product = new(),
            CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            })
        };

        if (id == null || id == 0)
        {
            return View(productVM);
        }
        else
        {
            productVM.Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
            productVM.Product.Images = _unitOfWork.Images.GetAll(i => i.ProductId == productVM.Product.Id).ToList(); // Get images
            return View(productVM);
        }
    }
    // 7/15/2024
    //   [HttpPost]
    //[ValidateAntiForgeryToken]
    //public IActionResult Upsert(ProductVM obj, IFormFile? file)
    //{

    //	if (ModelState.IsValid)
    //	{
    //		string wwwRootPath = _hostEnvironment.WebRootPath;
    //		if (file != null)
    //		{
    //			string fileName = Guid.NewGuid().ToString();
    //			var uploads = Path.Combine(wwwRootPath, @"images\products");
    //			var extension = Path.GetExtension(file.FileName);

    //			if (obj.Product.ImageUrl != null)
    //			{
    //				var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
    //				if (System.IO.File.Exists(oldImagePath))
    //				{
    //					System.IO.File.Delete(oldImagePath);
    //				}
    //			}

    //			using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
    //			{
    //				file.CopyTo(fileStreams);
    //			}
    //			obj.Product.ImageUrl = @"\images\products\" + fileName + extension;

    //		}
    //		if (obj.Product.Id == 0)
    //		{
    //			_unitOfWork.Product.Add(obj.Product);
    //		}
    //		else
    //		{
    //			_unitOfWork.Product.Update(obj.Product);
    //		}
    //		_unitOfWork.Save();
    //		TempData["success"] = "Product created successfully";
    //		return RedirectToAction("Index");
    //	}
    //	return View(obj);
    //}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(ProductVM obj, List<IFormFile> files)
    {
        if (ModelState.IsValid)
        {
            if (obj.Product.Id == 0)
            {
                _unitOfWork.Product.Add(obj.Product);
                _unitOfWork.Save(); // Save to generate ProductId for new product
            }
            else
            {
                _unitOfWork.Product.Update(obj.Product);
                _unitOfWork.Save();
            }

            // Handle image upload after product is saved
            if (files != null && files.Count > 0)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                foreach (var file in files)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\products");
                    var extension = Path.GetExtension(file.FileName);

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }

                    var image = new Images
                    {
                        Url = @"\images\products\" + fileName + extension,
                        ProductId = obj.Product.Id // Use the correct ProductId
                    };
                    _unitOfWork.Images.Add(image);
                }
                _unitOfWork.Save();
            }

            TempData["success"] = "Product created/updated successfully";
            return RedirectToAction("Index");
        }
        return View(obj);
    }

    #region API CALLS
    [HttpGet]
    public IActionResult GetAll()
    {
        var productList = _unitOfWork.Product.GetAll(includeProperties: "Category");
        return Json(new { data = productList });
    }

    //POST
    //[HttpDelete]
    //public IActionResult Delete(int? id)
    //{
    //    var obj = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
    //    if (obj == null)
    //    {
    //        return Json(new { success = false, message = "Error while deleting" });
    //    }

    //    var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
    //    if (System.IO.File.Exists(oldImagePath))
    //    {
    //        System.IO.File.Delete(oldImagePath);
    //    }

    //    _unitOfWork.Product.Remove(obj);
    //    _unitOfWork.Save();
    //    return Json(new { success = true, message = "Delete Successful" });

    //}
    // 7/15/2024
    [HttpDelete]
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == id, includeProperties: "Images");
        if (product == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }

        // Delete associated images
        foreach (var image in product.Images)
        {
            var imagePath = Path.Combine(_hostEnvironment.WebRootPath, image.Url.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            _unitOfWork.Images.Remove(image);
        }

        _unitOfWork.Product.Remove(product);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Delete Successful" });
    }

    #endregion
}
