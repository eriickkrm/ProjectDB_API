using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using ProjectDB_API.Models;

namespace ProjectDB_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("Register")]
        public IActionResult Register([FromBody] RegisterUserRequest request)
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            using (SqlCommand cmd = new SqlCommand("sp_RegisterUser", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@FirstName", request.FirstName);
                cmd.Parameters.AddWithValue("@LastName", request.LastName);
                cmd.Parameters.AddWithValue("@Email", request.Email);
                cmd.Parameters.AddWithValue("@Password", request.Password);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return Ok("Usuario registrado exitosamente.");
                }
                catch (SqlException ex)
                {
                    // Si el error viene de un usuario duplicado (correo ya existente)
                    if (ex.Message.Contains("correo electrónico"))
                        return Conflict("Ya existe un usuario registrado con ese correo electrónico.");

                    return StatusCode(500, $"Error interno: {ex.Message}");
                }
            }
        }
    }
}
