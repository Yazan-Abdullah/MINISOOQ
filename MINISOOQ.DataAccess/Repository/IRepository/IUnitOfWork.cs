using MINISOOQ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MINISOOQ.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category {  get; }
        IProductRepository Product { get; }
        ICompanyRepository Company {  get; }
        IShoppingCartRepository ShoppingCart {  get; }
        IFavoriteRepository Favorite { get; }
        IApplicationUserRepository ApplicationUser {  get; }
        IOrderDetailRepository OrderDetail {  get; }
        IOrderHeaderRepository OrderHeader {  get; }
        IImageRepository Images { get; }
        void Save();
		List<Product> GetAllProducts();
	}
}
