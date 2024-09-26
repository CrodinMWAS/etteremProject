// using Api.Database;
// using Api.Models;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using MySql.Data.MySqlClient;
//
// namespace Api.Controllers;
//
// [Route("api/v1/")]
// [ApiController]
// public class OrderController : ControllerBase
// {
//     private readonly DatabaseContext _databaseContext;
//
//     public OrderController(DatabaseContext databaseContext)
//     {
//         _databaseContext = databaseContext;
//     }
//
//     [HttpPost("placeOrder")]
//     public async Task<IActionResult> PlaceOrder([FromBody] OrderRequest request)
//     {
//         
//     }
//
// }