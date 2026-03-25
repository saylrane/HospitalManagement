using HospitalManagement.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        /// <summary>
        /// Login user and get JWT token
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                    return BadRequest(new { message = "Email and password are required" });

                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                    return Unauthorized(new { message = "Invalid email or password" });

                var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
                if (!result.Succeeded)
                    return Unauthorized(new { message = "Invalid email or password" });

                var token = await GenerateJwtTokenAsync(user);

                return Ok(new LoginResponseDto
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Token = token,
                    Message = "Login successful"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Register a new patient
        /// </summary>
        [HttpPost("register/patient")]
        [AllowAnonymous]
        public async Task<ActionResult<RegisterResponseDto>> RegisterPatient([FromBody] RegisterPatientRequestDto dto)
        {
            try
            {
                if (await _userManager.FindByEmailAsync(dto.Email) != null)
                    return BadRequest(new { message = "Email already exists" });

                var user = new IdentityUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors);
                    return BadRequest(new { message = $"Registration failed: {errors}" });
                }

                await _userManager.AddToRoleAsync(user, "Patient");

                return Ok(new RegisterResponseDto
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Role = "Patient",
                    Message = "Patient registered successfully. You can now login."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Register a new doctor
        /// </summary>
        [HttpPost("register/doctor")]
        [AllowAnonymous]
        public async Task<ActionResult<RegisterResponseDto>> RegisterDoctor([FromBody] RegisterDoctorRequestDto dto)
        {
            try
            {
                if (await _userManager.FindByEmailAsync(dto.Email) != null)
                    return BadRequest(new { message = "Email already exists" });

                var user = new IdentityUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors);
                    return BadRequest(new { message = $"Registration failed: {errors}" });
                }

                await _userManager.AddToRoleAsync(user, "Doctor");

                return Ok(new RegisterResponseDto
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Role = "Doctor",
                    Message = "Doctor registered successfully. You can now login."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Create admin user (Admin only)
        /// </summary>
        [HttpPost("create-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RegisterResponseDto>> CreateAdmin([FromBody] CreateAdminRequestDto dto)
        {
            try
            {
                if (await _userManager.FindByEmailAsync(dto.Email) != null)
                    return BadRequest(new { message = "Email already exists" });

                var user = new IdentityUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors);
                    return BadRequest(new { message = $"Failed to create admin: {errors}" });
                }

                await _userManager.AddToRoleAsync(user, "Admin");

                return Ok(new RegisterResponseDto
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Role = "Admin",
                    Message = "Admin created successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Logout user
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logout successful" });
        }

        // Helper method to generate JWT token
        private async Task<string> GenerateJwtTokenAsync(IdentityUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"] ?? "your-secret-key-here-min-32-characters"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "hospital-api",
                audience: _configuration["Jwt:Audience"] ?? "hospital-users",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // DTOs
    public class LoginRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponseDto
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
    }

    public class RegisterPatientRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterDoctorRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class CreateAdminRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterResponseDto
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Message { get; set; }
    }
}
