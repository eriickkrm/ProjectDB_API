using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProjectDB_API.Data;
using ProjectDB_API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectDB_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public LoginController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var emailParam = new SqlParameter("@Email", request.Email);
            var passwordParam = new SqlParameter("@Password", request.Password);
            var browserParam = new SqlParameter("@Browser", request.Browser);
            var publicIpParam = new SqlParameter("@PublicIP", request.PublicIP);

            try
            {
                var result = await _context.Set<LoginResult>()
                    .FromSqlRaw("EXEC sp_LoginUser @Email, @Password, @Browser, @PublicIP",
                        emailParam, passwordParam, browserParam, publicIpParam)
                    .ToListAsync();

                var login = result.FirstOrDefault();

                if (login == null || login.Id == 0)
                    return NotFound(new { message = login?.Mensaje ?? "Email o contraseña incorrecta" });

                var token = GenerateJwtToken(login.Email ?? "", request.Browser, request.PublicIP, request.Password);

                return Ok(new
                {
                    message = "Inicio de sesión exitoso",
                    token,
                    user = login
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        private string GenerateJwtToken(string email, string browser, string publicIP, string password)
        {
            var jwtSection = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iss, jwtSection["Issuer"]!),
                new Claim(JwtRegisteredClaimNames.Aud, jwtSection["Audience"]!),
                new Claim("browser", browser),
                new Claim("publicIP", publicIP),
                new Claim("password", password)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Browser { get; set; }
        public string PublicIP { get; set; }
    }

    public class LoginResult
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string Mensaje { get; set; } = "";
    }
}
