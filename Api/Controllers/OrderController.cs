using Api.Database;
using Api.Models;
using Api.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace Api.Controllers;

[Route("api/v1/")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly DatabaseContext _databaseContext;
    public OrderController(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }
    
    [HttpPost("order")]
    public async Task<IActionResult> PlaceOrder([FromBody] OrderRequest request)
    {
        var connection = _databaseContext.Database.GetDbConnection();
        await connection.OpenAsync();
        try
        {
            if (request == null || request.Dishes == null || !request.Dishes.Any())
            {
                return BadRequest("Invalid order request");
            }
            
            List<MySqlCommand> updateCommands = new List<MySqlCommand>();

            foreach (var dish in request.Dishes)
            {
                var getIngredientsQuery = "SELECT stock.ingredientId, stock.Quantity, dishIngredients.AmountNeeded AS UsedQuantity FROM Dishes INNER JOIN dishIngredients ON Dishes.dishId = dishIngredients.dishId INNER JOIN stock ON dishIngredients.ingredientId = stock.ingredientId WHERE Dishes.dishId = @DishId";
                var command = connection.CreateCommand();
                command.CommandText = getIngredientsQuery;
                command.Parameters.Add(new MySqlParameter("@DishId", dish.DishId));
                
                var reader = await command.ExecuteReaderAsync();
                
                List<(int IngredientId, float AvailableQuantity, float AmountNeeded)> ingredientData = new();

                while (await reader.ReadAsync())
                {
                    ingredientData.Add((reader.GetInt32(0), reader.GetFloat(1), reader.GetFloat(2)));
                }

                await reader.CloseAsync();

                foreach (var (ingredientId, availableQuantity, amountNeeded) in ingredientData)
                {
                    var totalNeededQuantity = amountNeeded * dish.Quantity;

                    if (availableQuantity < totalNeededQuantity)
                    {
                        throw new InvalidCastException($"Not enough stock for ingredient {ingredientId}");
                    }
                    
                    var updateStockQuery = "UPDATE stock SET Quantity = Quantity - @TotalNeeded WHERE IngredientId = @IngredientId";
                    
                    var updateCommand = connection.CreateCommand();
                    updateCommand.CommandText = updateStockQuery;
                    updateCommand.Parameters.Add(new MySqlParameter("@TotalNeeded", totalNeededQuantity));
                    updateCommand.Parameters.Add(new MySqlParameter("@IngredientId", ingredientId));
                    
                    updateCommands.Add((MySqlCommand)updateCommand);
                }
            }
            
            foreach (var updateCommand in updateCommands)
            {
                await updateCommand.ExecuteNonQueryAsync();
            }

            // Insert the order into Orders table without TotalAmount yet, as we will calculate it below
            var insertOrderQuery = "INSERT INTO Orders (UserId, OrderDate, Status) VALUES(@UserId, @OrderDate, @Status)";
            
            var insertOrderCommand = connection.CreateCommand();
            insertOrderCommand.CommandText = insertOrderQuery;
            insertOrderCommand.Parameters.Add(new MySqlParameter("@UserId", request.UserId));
            insertOrderCommand.Parameters.Add(new MySqlParameter("@OrderDate", DateTime.Now));
            insertOrderCommand.Parameters.Add(new MySqlParameter("@Status", "Pending"));

            await insertOrderCommand.ExecuteNonQueryAsync();
            
            // Get the last inserted order ID
            var getLastInsertedIdQuery = "SELECT LAST_INSERT_ID()";
            var getLastInsertedIdCommand = connection.CreateCommand();
            getLastInsertedIdCommand.CommandText = getLastInsertedIdQuery;
            
            var orderId = Convert.ToInt32(await getLastInsertedIdCommand.ExecuteScalarAsync());

            // Initialize total order price
            decimal totalOrderPrice = 0;
            
            // Loop through the dishes again to calculate the total price and insert into OrderDetails
            foreach (var dish in request.Dishes)
            {
                // Get the price of the dish from the Dishes table
                var getDishPriceQuery = "SELECT Price FROM Dishes WHERE DishId = @DishId";
                
                var getPriceCommand = connection.CreateCommand();
                getPriceCommand.CommandText = getDishPriceQuery;
                getPriceCommand.Parameters.Add(new MySqlParameter("@DishId", dish.DishId));
                
                var dishPrice = Convert.ToDecimal(await getPriceCommand.ExecuteScalarAsync());
                
                // Calculate the total price for this dish (price per dish * quantity ordered)
                var dishTotalPrice = dishPrice * dish.Quantity;
                
                // Add this dish's total price to the overall order total
                totalOrderPrice += dishTotalPrice;
                
                // Insert the dish details into OrderDetails, including the price at the time of the order
                var insertOrderDetailQuery = "INSERT INTO OrderDetails (OrderId, DishId, Quantity, PriceAtOrder) VALUES (@OrderId, @DishId, @Quantity, @PriceAtOrder)";
                
                using var insertOrderDetailCommand = connection.CreateCommand();
                insertOrderDetailCommand.CommandText = insertOrderDetailQuery;
                insertOrderDetailCommand.Parameters.Add(new MySqlParameter("@OrderId", orderId));
                insertOrderDetailCommand.Parameters.Add(new MySqlParameter("@DishId", dish.DishId));
                insertOrderDetailCommand.Parameters.Add(new MySqlParameter("@Quantity", dish.Quantity));
                insertOrderDetailCommand.Parameters.Add(new MySqlParameter("@PriceAtOrder", dishTotalPrice)); // Insert the total price for this dish
                
                await insertOrderDetailCommand.ExecuteNonQueryAsync();
            }

            // Update the order's TotalAmount in the Orders table
            var updateOrderTotalQuery = "UPDATE Orders SET TotalAmount = @TotalAmount WHERE OrderId = @OrderId";
            
            using var updateOrderTotalCommand = connection.CreateCommand();
            updateOrderTotalCommand.CommandText = updateOrderTotalQuery;
            updateOrderTotalCommand.Parameters.Add(new MySqlParameter("@TotalAmount", totalOrderPrice));
            updateOrderTotalCommand.Parameters.Add(new MySqlParameter("@OrderId", orderId));
            
            await updateOrderTotalCommand.ExecuteNonQueryAsync();

            return Ok(new { Message = "Order successful", OrderId = orderId });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, $"Internal server error: {e.Message}");
        }
    }


}