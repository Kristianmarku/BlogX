using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using BlogX.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace BlogX.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly SignInManager<UserModel> _signInManager;
        private readonly UserManager<UserModel> _userManager;
        private readonly IConfiguration _config;

        public AuthApiController(IConfiguration _config, SignInManager<UserModel> signInManager, UserManager<UserModel> userManager)
        {
            this._config = _config;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            try
            {
                string userName = loginModel.UserName;
                string password = loginModel.Password;
                bool rememberMe = false;

                // Validate the input
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                {
                    return BadRequest(new { success = false, message = "Invalid login data" });
                }

                // Login logic
                var result = await _signInManager.PasswordSignInAsync(userName, password, rememberMe, lockoutOnFailure: false);


                if (result.Succeeded)
                {
                    // Fetch User
                    var user = await _userManager.FindByNameAsync(userName);

                    // Generate a JWT
                    var token = GenerateToken(user);

                    // Authentication successful
                    return Ok(new { success = true, message = "Login successful", user = GetUserDetails(user), token });
                }
                else
                {
                    // Authentication failed
                    return BadRequest(new { success = false, message = "Invalid login attempt" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            try
            {
                // Validate the input
                if (registerModel == null || string.IsNullOrEmpty(registerModel.UserName) || string.IsNullOrEmpty(registerModel.Password))
                {
                    return BadRequest(new { success = false, message = "Invalid registration data" });
                }

                // Check if the user already exists
                var existingUser = await _userManager.FindByNameAsync(registerModel.UserName);
                if (existingUser != null)
                {
                    return BadRequest(new { success = false, message = "Username is already taken" });
                }

                // Create a new user
                var newUser = new UserModel
                {
                    UserName = registerModel.UserName,
                    Email = registerModel.UserName,
                    FirstName = registerModel.FirstName,
                    LastName = registerModel.LastName
                };

                var result = await _userManager.CreateAsync(newUser, registerModel.Password);

                if (result.Succeeded)
                {
                    // Assign the default role to the user
                    await _userManager.AddToRoleAsync(newUser, "Member");

                    // Generate a JWT
                    var token = GenerateToken(newUser);

                    // Registration successful
                    return Ok(new { success = true, message = "Registration successful", user = GetUserDetails(newUser), token });
                }
                else
                {
                    // Registration failed
                    return BadRequest(new { success = false, message = "Registration failed", errors = result.Errors });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        private object GetUserDetails(UserModel user)
        {
            return new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.FirstName,
                user.LastName,
                user.PhoneNumber
            };
        }

        private string GenerateToken(UserModel user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"], null,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
