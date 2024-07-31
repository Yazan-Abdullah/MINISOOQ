using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MINISOOQ.Models.ViewModels
{
	public class ProductsViewModel
	{
		
			public List<Product> MensProducts { get; set; }
			public List<Product> WomensProducts { get; set; }
			public List<Product> KidsProducts { get; set; }
            public ShoppingCart ShoppingCart { get; set; }
		    public Favorite Favorite { get; set; }
    }
}
