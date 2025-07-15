using Microsoft.EntityFrameworkCore;

namespace ProjectDB_API.Models
{
    [Keyless]
    public class MenuItemResult
    {
        public string Name { get; set; }
        public string Route { get; set; }
        public string Icon { get; set; }
    }
}