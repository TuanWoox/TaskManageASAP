using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebApplication1.Data;
using WebApplication1.DTOs.UserDTO;
using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public class UserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public User CreateUser(CreateUserDTO createUserDTO)
        {
            User newUser = ConvertCreateDTOTOne(createUserDTO);
            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();
            return newUser;
        }
        public User FindUser(LoginUserDTO loginUserDTO)
        {
            return _dbContext.Users.Include(t => t.Role).FirstOrDefault(t => t.UserName == loginUserDTO.UserName);
        }
        private User ConvertCreateDTOTOne(CreateUserDTO createUserDTO)
        {
            return new User
            {
                UserName = createUserDTO.UserName,
                Email = createUserDTO.Email,
                PasswordHash = createUserDTO.PasswordHash,
                RoleId = 1
            };
        }
    }
}
