namespace Api.Models;

public class IngredientModel
{
    public string IngredientName { get; set; }
    public float AmountNeeded { get; set; }
    public string Unit { get; set; }
    public float InStock { get; set; }
}