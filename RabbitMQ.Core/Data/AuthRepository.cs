using AutoMapper;
using BrowserTextRPG.DTOModel.User;
using BrowserTextRPG.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Logging;
using Nancy.Json;

namespace BrowserTextRPG.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IMapper _mapper;
        private readonly DataContext _dbContext;
        private readonly IConfiguration _appConfig;
        private readonly ILogger<AuthRepository> _logger;
        private readonly JavaScriptSerializer _jsonSerializer = new JavaScriptSerializer();

        public AuthRepository(
            DataContext dbContext,
            IMapper mapper,
            IConfiguration config,
            ILogger<AuthRepository> logger)
        {
            this._dbContext = dbContext;
            this._mapper = mapper;
            this._appConfig = config;
            this._logger = logger;
        }

        public async Task<GatewayResponse<string>> Login(string username, string password)
        {
            this._logger.LogInformation($"Get 'Login' request: {{ username: {username}, password {password} }}.");

            var response = new GatewayResponse<string>();

            try
            {
                var user = await this._dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Name == username);

                if (user == null)
                {
                    response.Fault = new Fault
                    {
                        ErrorCode = 1003,
                        ErrorMessage = "User not found"
                    };

                    this._logger.LogWarning($"Request failed due to {response.Fault.ErrorMessage}.");

                    return response;
                }

                bool isPasswordRight = VerifyPasswordHash(password, user.PasswordSalt, user.PasswordHash);
                if (!isPasswordRight)
                {
                    response.Fault = new Fault
                    {
                        ErrorCode = 1002,
                        ErrorMessage = "Wrong UserName or Password"
                    };

                    this._logger.LogWarning($"Request failed due to {response.Fault.ErrorMessage}.");

                    return response;
                }

                response.Data = CreateToken(user);
            }
            catch (Exception e)
            {
                response.Fault = new Fault
                {
                    ErrorCode = e.HResult,
                    ErrorMessage = e.Message
                };

                this._logger.LogError($"Error with code {e.HResult}: {e.Message}.");
            }

            this._logger.LogInformation($"User succesfully logged in: {{ {this._jsonSerializer.Serialize(response.Data)} }}");

            return response;
        }

        public async Task<GatewayResponse<int>> Register(GetUserDto user)
        {
            this._logger.LogInformation($"Get 'Register' request: {{ {this._jsonSerializer.Serialize(user)} }}.");

            var response = new GatewayResponse<int>();

            var userExist = await UserExist(user.Name);

            if (userExist.Data)
            {
                response.Fault = new Fault
                {
                    ErrorCode = 1001,
                    ErrorMessage = "User already exist"
                };

                this._logger.LogWarning($"Request failed due to {response.Fault.ErrorMessage}.");

                return response;
            }

            try
            {
                Utility.CreatePasswordHash(user.Password, out byte[] passwordSalt, out byte[] passwordHash);

                var fullUser = _mapper.Map<GetUserDto, User>(user);
                fullUser.PasswordSalt = passwordSalt;
                fullUser.PasswordHash = passwordHash;

                this._dbContext.Users.Add(fullUser);
                await this._dbContext.SaveChangesAsync();

                response.Data = fullUser.Id;
            }
            catch (Exception e)
            {
                response.Fault = new Fault
                {
                    ErrorCode = e.HResult,
                    ErrorMessage = e.Message
                };

                this._logger.LogError($"Error with code {e.HResult}: {e.Message}.");
            }

            this._logger.LogInformation($"User succesfully created in the system: {{ {this._jsonSerializer.Serialize(response.Data)} }}");

            return response;
        }

        public async Task<GatewayResponse<bool>> UserExist(string username)
        {
            var response = new GatewayResponse<bool>();

            try
            {
                var userExist = await this._dbContext.Users.AnyAsync(u => u.Name.ToLower().Equals(username.ToLower()));

                response.Data = userExist;
            }
            catch (Exception e)
            {
                response.Fault = new Fault
                {
                    ErrorCode = e.HResult,
                    ErrorMessage = e.Message
                };
            }

            return response;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordSalt, byte[] passwordHash)
        {
            byte[] generatedhash;

            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                generatedhash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

            bool isEqual = Enumerable.SequenceEqual(generatedhash, passwordHash);

            return isEqual;
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role.Name),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_appConfig.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
