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
                
                var query = "SELECT DishId, DishName, Description, Allergens, Price, ImageLink FROM Dishes";
                
                var command = _databaseContext.Database.GetDbConnection().CreateCommand();
                command.CommandText = query;
                
                await _databaseContext.Database.OpenConnectionAsync();
                
                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var dish = new DishModel
                    {
                        DishId = reader.GetInt32(0),
                        DishName = reader.GetString(1),
                        Description = reader.GetString(2),
                        Allergens = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Price = reader.GetInt32(4),
                        ImageUrl = reader.GetString(5)
                    };

                    dishes.Add(dish);
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