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
	public class FavoriteRepository : Repository<Favorite>, IFavoriteRepository
	{
		private ApplicationDbContext _db;

		public FavoriteRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		public int DecrementCount(Favorite favorite, int count)
		{
			favorite.Count -= count;
			return favorite.Count;
		}

		public int IncrementCount(Favorite favorite, int count)
		{
			favorite.Count += count;
			return favorite.Count;
		}
        public IEnumerable<Favorite> GetFavoritesWithProducts(string userId)
        {
            return _db.Favorites
                .Include(f => f.Product)
                    .ThenInclude(p => p.Images)  // Ensure this path is correct
                .Where(f => f.ApplicationUserId == userId)
                .ToList();
        }
    }
}

