using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MINISOOQ.Models
{
	public class Favorite
	{
		[Key]
		public int Id { get; set; }
		public int ProductId { get; set; }
		public string ApplicationUserId { get; set; }
		[ForeignKey("ApplicationUserId")]
		[ValidateNever]
		public ApplicationUser ApplicationUser { get; set; }
		[ForeignKey("ProductId")]
		public Product Product { get; set; }
		public int Count { get; set; }
	}
}
