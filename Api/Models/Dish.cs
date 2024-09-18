using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{
	[Table("dishes")]
	public class Dish
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("dishId")]
		public int DishId { get; set; }
		
		[Required]
		[Column("dishName")]
		public string DishName { get; set; }

		public ICollection<Order> Orders { get; set; }
		public ICollection<Ingredient> Ingredients { get; set; }


	}
}
