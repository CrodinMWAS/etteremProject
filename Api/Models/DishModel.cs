using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models;

public class DishModel
{
    [Key] public int DishId { get; set; }
    public string DishName { get; set; }
    public string Description { get; set; }
    public string Allergens { get; set; }
    public int Price { get; set; }
    public string ImageUrl { get; set; }
    public string Category { get; set; }
    [NotMapped]
    public List<IngredientModel> Ingredients { get; set; }

}