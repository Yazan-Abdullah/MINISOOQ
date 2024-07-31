using MINISOOQ.DataAccess.Repository.IRepository;
using MINISOOQ.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MINISOOQ.Models;
using MINISOOQ.Utility;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace MINISOOQ.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class FavoritesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public FavoritesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var favoriteVM = new FavoriteVM
            {
                ListFavorite = _unitOfWork.Favorite.GetFavoritesWithProducts(claim.Value)
            };

            return View(favoriteVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToFavorite(int productId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var favorite = _unitOfWork.Favorite.GetFirstOrDefault(u => u.ApplicationUserId == claim.Value && u.ProductId == productId);

            if (favorite == null)
            {
                favorite = new Favorite
                {
                    ProductId = productId,
                    ApplicationUserId = claim.Value,
                    Count = 1
                };

                _unitOfWork.Favorite.Add(favorite);
            }
            else
            {
                _unitOfWork.Favorite.IncrementCount(favorite, 1);
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Remove(int favoriteId)
        {
            var favorite = _unitOfWork.Favorite.GetFirstOrDefault(u => u.Id == favoriteId);

            if (favorite != null)
            {
                _unitOfWork.Favorite.Remove(favorite);
                _unitOfWork.Save();

                // Update session variable with the count of remaining favorites
                var count = _unitOfWork.Favorite.GetAll(u => u.ApplicationUserId == favorite.ApplicationUserId).Count();
                HttpContext.Session.SetInt32(SD.SessionFavorite, count);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
