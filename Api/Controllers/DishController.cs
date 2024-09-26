using Api.Database;
using Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace Api.Controllers;

[Route("api/v1/")]
[ApiController]
public class DishController : ControllerBase
{
    private readonly DatabaseContext _databaseContext;

    public DishController(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    [HttpGet("dishes")]
    public async Task<IActionResult> GetDishes()
    {
        try
            {
                var dishes = new List<DishModel>();
                var query = "SELECT d.DishId, d.DishName, d.Description, d.Allergens, d.Price, d.ImageLink, di.AmountNeeded, i.IngredientName, i.Unit, s.Quantity FROM Dishes d left join DishIngredients di on d.DishId = di.DishId\nleft join ingredients i on di.IngredientId = i.IngredientId left join Stock s on i.IngredientId = s.IngredientId";
                
                var command = _databaseContext.Database.GetDbConnection().CreateCommand();
                command.CommandText = query;
                
                await _databaseContext.Database.OpenConnectionAsync();
                
                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    int dishId = reader.GetInt32(0);
                    
                    var dish = dishes.FirstOrDefault(d => d.DishId == dishId);

                    if (dish == null)
                    {
                        dish = new DishModel
                        {
                            DishId = dishId,
                            DishName = reader.GetString(1),
                            Description = reader.GetString(2),
                            Allergens = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Price = reader.GetInt32(4),
                            ImageUrl = reader.GetString(5),
                            Ingredients = new List<IngredientModel>()
                        };
                        dishes.Add(dish);
                    }
                    
                    dish.Ingredients.Add(new IngredientModel
                    {
                        IngredientName = reader.GetString(7),
                        AmountNeeded = reader.GetFloat(6),
                        Unit = reader.GetString(8),
                        InStock = reader.GetFloat(9)
                    });
                }
                
                await _databaseContext.Database.CloseConnectionAsync();
                
                return Ok(dishes);
            }
            catch (MySqlException ex)
            {
                return StatusCode(500, new { message = "An error occurred while accessing the database.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
            finally
            {
                if (_databaseContext.Database.GetDbConnection().State == System.Data.ConnectionState.Open)
                {
                    await _databaseContext.Database.CloseConnectionAsync();
                }
            }
    }

}