using WebApplication1.DTOs.UserDTO;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Services
{
    public class UserService
    {
        private readonly UserRepository userRepository;

        public UserService(UserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        public UserDTO CreateUser(CreateUserDTO createUserDTO)
        {
            return ConvertsOneToDTO(userRepository.CreateUser(createUserDTO));  
        }
        public User AuthenticateUser(LoginUserDTO loginUserDTO)
        {
            User foundUser = userRepository.FindUser(loginUserDTO);
            if (foundUser != null)
            {
                bool isValid = BCrypt.Net.BCrypt.Verify(loginUserDTO.Password, foundUser.PasswordHash);
                return isValid ? foundUser : null;
            }
            return null; 
        }
        private UserDTO ConvertsOneToDTO(User newUser) {
            return new UserDTO
            {
                Id = newUser.Id,
                UserName = newUser.UserName,
                Email = newUser.Email,
                RoleId = newUser.RoleId,
            };
        }

    }
}
