namespace Api.Models;

public class IngredientModel
{
    public string IngredientName { get; set; }
    public decimal AmountNeeded { get; set; }
    public string Unit { get; set; }
    public decimal InStock { get; set; }
}