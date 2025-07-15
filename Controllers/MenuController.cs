using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProjectDB_API.Data;
using ProjectDB_API.Models;

namespace ProjectDB_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MenuController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Menu/1,2,3
        [HttpGet("{roleIds}")]
        public async Task<IActionResult> GetMenuItemsByRoles(string roleIds)
        {
            if (string.IsNullOrWhiteSpace(roleIds))
                return BadRequest(new { error = "El parámetro roleIds es requerido." });

            try
            {
                var param = new SqlParameter("@Role_IDs", roleIds);

                var items = await _context.MenuItemResults
                    .FromSqlRaw("EXEC sp_GetMenuByRole @Role_IDs", param)
                    .ToListAsync();

                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
