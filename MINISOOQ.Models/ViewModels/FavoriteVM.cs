using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MINISOOQ.Models.ViewModels
{
	public class FavoriteVM
	{
		public IEnumerable<Favorite> ListFavorite { get; set; }

		public Product product { get; set; }
	}
}
