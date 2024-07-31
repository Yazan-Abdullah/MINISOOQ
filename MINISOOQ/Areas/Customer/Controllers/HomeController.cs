using MINISOOQ.DataAccess;
using MINISOOQ.DataAccess.Repository.IRepository;
using MINISOOQ.Models;
using MINISOOQ.Models.ViewModels;
using MINISOOQ.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stripe;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace MINISOOQ.Controllers;
[Area("Customer")]
public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ApplicationDbContext _context;
	public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, ApplicationDbContext context)
	{
		_logger = logger;
		_unitOfWork = unitOfWork;
		_context = context;
	}

	public IActionResult Index()
	{
		var allProducts = _unitOfWork.Product.GetAll(includeProperties: "Category,Images").ToList();

		var viewModel = new ProductsViewModel
		{
			MensProducts = allProducts.Where(p => p.Category.Name == "Men's").ToList(),
			WomensProducts = allProducts.Where(p => p.Category.Name == "Women's").ToList(),
			KidsProducts = allProducts.Where(p => p.Category.Name == "Kid's").ToList(),
			ShoppingCart = new ShoppingCart()
		};

		return View(viewModel);
	}
	public IActionResult AddToCart()
	{
		return RedirectToAction(nameof(Index));
	}
	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize]
	public IActionResult AddToCart(int productId)
	{
		var claimsIdentity = (ClaimsIdentity)User.Identity;
		var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
		var shoppingCart = new ShoppingCart
		{
			ProductId = productId,
			ApplicationUserId = claim.Value,
			Count = 1 // Default to 1 for now
		};

		ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
			u => u.ApplicationUserId == claim.Value && u.ProductId == shoppingCart.ProductId);

		if (cartFromDb == null)
		{
			_unitOfWork.ShoppingCart.Add(shoppingCart);
			_unitOfWork.Save();
			HttpContext.Session.SetInt32(SD.SessionCart,
				_unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count);
		}
		else
		{
			_unitOfWork.ShoppingCart.IncrementCount(cartFromDb, 1);
			_unitOfWork.Save();
		}
		return RedirectToAction(nameof(Index));
	}
	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize]
	public IActionResult AddToFavorite(int productId)
	{
		var claimsIdentity = (ClaimsIdentity)User.Identity;
		var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
		var favorite = new Favorite
		{
			ProductId = productId,
			ApplicationUserId = claim.Value,
			Count = 1 // Default to 1 for now
		};

		Favorite favoriteFromDb = _unitOfWork.Favorite.GetFirstOrDefault(
			u => u.ApplicationUserId == claim.Value && u.ProductId == favorite.ProductId);

		if (favoriteFromDb == null)
		{
			_unitOfWork.Favorite.Add(favorite);
			_unitOfWork.Save();
			HttpContext.Session.SetInt32(SD.SessionFavorite,
				_unitOfWork.Favorite.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count);
		}
		else
		{
			_unitOfWork.Favorite.IncrementCount(favoriteFromDb, 1);
			_unitOfWork.Save();
		}
		return RedirectToAction(nameof(Index));
	}
	public IActionResult AddToFavorite()
	{
		return RedirectToAction(nameof(Index));
	}

	public IActionResult Details(int productId)
	{
		ShoppingCart cartObj = new()
		{
			Count = 1,
			ProductId = productId,
			Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == productId, includeProperties: "Category,Images"),
		};

		return View(cartObj);
	}


	//[HttpPost]
	//[ValidateAntiForgeryToken]
	//[Authorize]
	//public IActionResult Details(ShoppingCart shoppingCart)
	//{
	//    var claimsIdentity = (ClaimsIdentity)User.Identity;
	//    var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
	//    shoppingCart.ApplicationUserId = claim.Value;

	//    ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
	//        u => u.ApplicationUserId == claim.Value && u.ProductId == shoppingCart.ProductId);

	//    if (cartFromDb == null) {

	//        _unitOfWork.ShoppingCart.Add(shoppingCart);
	//        _unitOfWork.Save();
	//        HttpContext.Session.SetInt32(SD.SessionCart,
	//            _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count);
	//    }
	//    else
	//    {
	//        _unitOfWork.ShoppingCart.IncrementCount(cartFromDb, shoppingCart.Count);
	//        _unitOfWork.Save();
	//    }
	//    return RedirectToAction(nameof(Index));
	//}
	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize]
	public IActionResult Details(ShoppingCart shoppingCart)
	{
		var claimsIdentity = (ClaimsIdentity)User.Identity;
		var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
		shoppingCart.ApplicationUserId = claim.Value;
		shoppingCart.Count = 1;
		ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
			u => u.ApplicationUserId == claim.Value && u.ProductId == shoppingCart.ProductId && u.Size == shoppingCart.Size && u.Color == shoppingCart.Color);

		if (cartFromDb == null)
		{

			_unitOfWork.ShoppingCart.Add(shoppingCart);
			_unitOfWork.Save();
			HttpContext.Session.SetInt32(SD.SessionCart,
				_unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count);
		}
		else
		{
			_unitOfWork.ShoppingCart.IncrementCount(cartFromDb, shoppingCart.Count);
			_unitOfWork.Save();
		}
		return RedirectToAction(nameof(Index));
	}

	public IActionResult Privacy()
	{
		return View();
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}
