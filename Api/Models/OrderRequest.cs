namespace Api.Models;

public class OrderRequest
{
    public int UserId { get; set; }
    public List<DishOrder> Dishes { get; set; }
}