using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace RoomService.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        Console.WriteLine(token);
        Console.WriteLine("THIS IS THE KEY " + (_configuration["AuthSettings:Secret"]));
        
        
        if (token != null)
        {
            try
            {
                AttachUserToContext(context, token);
            }
            catch
            {
                Console.WriteLine("Invalid token");
            }
        }

        await _next(context);
    }

   
    private void AttachUserToContext(HttpContext context, string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["AuthSettings:Secret"]);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["AuthSettings:Issuer"],
                ValidAudience = _configuration["AuthSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.FromMinutes(15)
            };

            // Validate the token
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            // Attach user claims to the context
            context.Items["User"] = principal;

            Console.WriteLine("Token successfully validated");
            Console.WriteLine($"Token Expiration: {((JwtSecurityToken)validatedToken).ValidTo}");
        }
        catch (SecurityTokenExpiredException ex)
        {
            Console.WriteLine("Token validation failed: Token is expired.");
            throw new UnauthorizedAccessException("Token is expired", ex);
        }
        catch (SecurityTokenException ex)
        {
            Console.WriteLine($"Token validation failed: {ex.Message}");
            throw new UnauthorizedAccessException("Token is invalid", ex);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            throw;
        }
    }


}
