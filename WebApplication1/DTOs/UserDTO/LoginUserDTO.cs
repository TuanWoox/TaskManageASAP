namespace WebApplication1.DTOs.UserDTO
{
    public record LoginUserDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
