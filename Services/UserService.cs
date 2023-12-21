using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TaskStore.Data;
using TaskStore.Model;

namespace TaskStore.Services
{
    public class UserService : IUserService
    {
        public readonly ApplicationDBContext _context;
        private IConfiguration _configuration;
        public UserService(ApplicationDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<ApiResponse> Login(AddUserRequest addUserRequest)
        {
            ApiResponse response = new() { statusCode = 200 };
            try
            {
                var user = await _context.Users.Where(x => x.Email == addUserRequest.Email && x.IsDeleted != true).FirstOrDefaultAsync();
                if (user == null)
                {
                    throw new Exception("User not found");
                }
                else
                {
                    if (!VerifyPasswordHash(addUserRequest.Password, user.PasswordHash, user.PasswordSalt))
                    {
                        throw new Exception("Wrong password.");
                    }
                    var token = GenerateToken(user);
                    response.data = JsonSerializer.Serialize(new { token = token, id = user.Id, username = user.Username, email = user.Email });
                }
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
        private string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], null,
                    expires: DateTime.Now.AddDays(3),
                    signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<ApiResponse> Register(AddUserRequest addUserRequest)
        {
            ApiResponse response = new() { statusCode = 200 };

            try
            {
                bool isUsernameDuplicate = await _context.Users.Where(x => x.Username == addUserRequest.Username && x.IsDeleted != true).AnyAsync();
                bool isEmailDuplicate = await _context.Users.Where(x => x.Email == addUserRequest.Email && x.IsDeleted != true).AnyAsync();
                if (isUsernameDuplicate)
                {
                    throw new Exception("duplicate user name");
                }
                if (isEmailDuplicate)
                {
                    throw new Exception("duplicate email");
                }
                else
                {
                    CreatePasswordHash(addUserRequest.Password, out byte[] passwordHash, out byte[] passwordSalt);
                    var newUser = new User()
                    {
                        Id = Guid.NewGuid(),
                        Username = addUserRequest.Username,
                        Email = addUserRequest.Email,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        CreatedDate = DateTime.Now,
                    };
                    await _context.Users.AddAsync(newUser);
                    await _context.SaveChangesAsync();
                    response.message = "User Created Sucessfully";
                }
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
