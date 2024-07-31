using MINISOOQ.DataAccess.Repository.IRepository;
using MINISOOQ.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MINISOOQ.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public List<Product> GetAllProduct()
		{
			return _db.Products.Include(p => p.Category).ToList();
		}

		public void Update(Product obj)
        {
            var objFromDb = _db.Products.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = obj.Title;
                objFromDb.Price = obj.Price;
                objFromDb.Description = obj.Description;
                objFromDb.CategoryId = obj.CategoryId;
                if (obj.Images != null)
                {
                    objFromDb.Images = obj.Images;  
                }
            }
        }
    }
}
