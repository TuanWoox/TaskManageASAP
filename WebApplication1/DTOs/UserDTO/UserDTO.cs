using WebApplication1.Models;

namespace WebApplication1.DTOs.UserDTO
{
    public record UserDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        
    }
}
