using MINISOOQ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MINISOOQ.DataAccess.Repository.IRepository
{
	public interface IFavoriteRepository : IRepository<Favorite>
	{
		int IncrementCount(Favorite favorite, int count);
		public IEnumerable<Favorite> GetFavoritesWithProducts(string userId);

        int DecrementCount(Favorite favorite, int count);
	}
}
