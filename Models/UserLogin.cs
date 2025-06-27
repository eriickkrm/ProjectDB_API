namespace ProjectDB_API.Models
{
    public class UserLogin
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime LoginTime { get; set; }
        public string Status { get; set; }
    }
}
