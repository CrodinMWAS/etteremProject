using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{
	[Table("stock")]
	public class Stock
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("stockId")]
		public int StockId { get; set; }

		[Required]
		[Column("stockName")]
		public string StockName { get; set; }

		[Required]
		[Column("amount")]
		public int Amount { get; set; }

		public ICollection<Ingredient> Ingredients { get; set; }

	}
}
