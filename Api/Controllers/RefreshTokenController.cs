using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Api.Service;
using Microsoft.Extensions.Caching.Memory;


namespace Api.Controllers;

[Route("api/v1/")]
[ApiController]
public class RefreshTokenController : ControllerBase
{
    private readonly IMemoryCache _memoryCache;
    private readonly TokenService _tokenService;

    public RefreshTokenController(IMemoryCache memoryCache, TokenService tokenService)
    {
        _memoryCache = memoryCache;
        _tokenService = tokenService;
    }
     
    [HttpPost("refreshToken")]
    public IActionResult RefreshToken([FromBody] TokenModel model)
    {
        if (_memoryCache.TryGetValue(model.RefreshToken, out string username))
        {
            string newAccessToken = _tokenService.GenerateJWTToken(username);
            
            string newRefreshToken = _tokenService.GenerateRefreshToken();
            _memoryCache.Set(newRefreshToken, username, TimeSpan.FromDays(7));
            _memoryCache.Remove(model.RefreshToken);

            return Ok(new 
            { 
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        return Unauthorized("Invalid or expired refresh token");
    }
}