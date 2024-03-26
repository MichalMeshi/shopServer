using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = "user";

    }
}
