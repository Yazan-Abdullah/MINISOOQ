using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MINISOOQ.Models
{
    public class Product
    {
		public int Id { get; set; }
		[Required]
		public string Title { get; set; }
		public string Description { get; set; }

		[Required]
		[Range(1, 10000)]
		[Display(Name = "Price for 1-50")]
		public double Price { get; set; }

		[Required]
		[Display(Name = "Category")]
		public int CategoryId { get; set; }
		[ForeignKey("CategoryId")]
		[ValidateNever]
		public Category Category { get; set; }

        public ICollection<Images> Images { get; set; } = new List<Images>();

    }
}
