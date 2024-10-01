// using Api.Database;
// using Api.Models;
// using Microsoft.AspNetCore.Http.HttpResults;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using MySql.Data.MySqlClient;
//
// namespace Api.Helpers;
//
// public class OrderHelper
// {
//     //1. Get the required amount of the ingredients that were used in the order
//     //2. Get the all the available ingredient quantities of that ingredient
//     //3. Decrease the quantity with the amount used in the order and update the database
//     //4. Insert the order contents in the order table
//     //5. Insert the orderDetail contents in the order details table
//     private readonly DatabaseContext _databaseContext;
//
//     public OrderHelper(DatabaseContext databaseContext)
//     {
//         _databaseContext = databaseContext;
//     }
//
//     public async void UpdateIngredients(OrderRequest request)
//     {
//         var connection = _databaseContext.Database.GetDbConnection();
//         connection.OpenAsync();
//         try
//         {
//             List<int> ingredients = new();
//             
//             //Gets the quantity of the ingredient avalaible for the current dish to make
//             var getUsedIngredientsQuery = "Select stock.Quantity FROM Dishes inner join dishIngredients on dishes.dishId = dishIngredients.dishId inner join ingredients on dishIngredients.ingredientId = ingredients.ingredientId inner join stock on ingredients.ingredientId = stock.ingredientId where dishes.dishId = @DishId";
//             
//             var command = _databaseContext.Database.GetDbConnection().CreateCommand();
//             command.CommandText = getUsedIngredientsQuery;
//
//             await connection.OpenAsync();
//             var reader = await command.ExecuteReaderAsync();
//             while (await reader.ReadAsync())
//             {
//                 foreach (var i in request.Dishes)
//                 {
//                     command.Parameters.Add(new MySqlParameter("@DishId", i.DishId));
//                     ingredients.Add(reader.GetInt32(0));
//                 }    
//             }
//             
//             connection.CloseAsync();
//         }
//         catch (Exception e)
//         {
//             Console.WriteLine(e);
//             throw;
//         }
//         
//         
//             
//             
//     } 
//     
// }