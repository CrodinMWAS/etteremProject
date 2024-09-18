using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{
	[Table("ingredients")]
	public class Ingredient
	{
		[Key]
		[Column("dishId", Order = 0)]
		public int DishId { get; set; }

		[Key]
		[Column("stockId", Order = 1)]
		public int StockId { get; set; }

		public Dish Dish { get; set; }
		public Stock Stock { get; set; }
	}
}
