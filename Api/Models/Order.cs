using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models

{
	[Table("orders")]
	public class Order
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("orderId")]
		public int OrderId { get; set; }

		[Required]
		[ForeignKey("User")]
		[Column("userId")]
		public int UserId { get; set; }

		[Required]
		[ForeignKey("Dish")]
		[Column("dishId")]
		public int DishId { get; set; }

		public User User { get; set; }
		public Dish Dish { get; set; }
	}
}
