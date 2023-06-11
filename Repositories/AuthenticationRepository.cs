using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using api_aspnetcore6.Repositories.Interfaces;
using api_aspnetcore6.Dtos;
using api_aspnetcore6.Models;
using api_aspnetcore6.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Web;

namespace api_aspnetcore6.Repositories
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly DatabaseContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISendMailService _mailService;

        public AuthenticationRepository(DatabaseContext dbContext, IConfiguration configuration,
        UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
        IHttpContextAccessor httpContextAccessor, ISendMailService mailService)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _mailService = mailService;
        }
        public async Task<ResponseMessage> Login(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByNameAsync(loginDTO.UserName);

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDTO.Password))
            {
                return new ResponseMessage(false, "User or password incorrect!");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var testrole = userRoles[0];

            var accessToken = await GenerateAcccessToken(user, userRoles);
            var refreshToken = GenerateRefreshToken();

            _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);

            await _userManager.UpdateAsync(user);

            return new ResponseMessage(true, "Login successfully", new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        public async Task<ResponseMessage> Register(RegisterDTO registerDTO)
        {
            var userExists = await _userManager.FindByNameAsync(registerDTO.UserName);
            var emailExists = await _userManager.FindByEmailAsync(registerDTO.Email);

            if (userExists != null || emailExists != null)
            {
                return new ResponseMessage(false, "User already exists");
            }

            var user = new ApplicationUser
            {
                UserName = registerDTO.UserName,
                Email = registerDTO.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
            {
                return new ResponseMessage(false, "User creation failed! Please check user details and try again.");
            }

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            };

            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            };

            var dtoRole = registerDTO.Role.ToLower();

            var isAdmin = dtoRole.Equals(UserRoles.Admin.ToLower());
            var isUser = dtoRole.Equals(UserRoles.User.ToLower());

            if (isAdmin && await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }

            if (isUser && await _roleManager.RoleExistsAsync(UserRoles.User))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            string encodedToken = HttpUtility.UrlEncode(token);

            var confirmationLink = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/confirmemail?token={encodedToken}&email={user.Email}";
            var toEmail = new string[] { user.Email };
            var subject = "Confirmation email link";
            var body = $"Please click on the following link to confirm email: <a href='{confirmationLink}'>Confirm Email</a>";

            var message = new MailMessage(toEmail, subject, body);

            await _mailService.SendEmailAsync(message);

            return new ResponseMessage(true, "User created successfully!");
        }

        public async Task<ResponseMessage> RefreshToken(TokenModel tokenModel)
        {

            var claimsPrincipal = GetPrincipalFromExpiredToken(tokenModel.AccessToken);
            if (claimsPrincipal == null)
            {
                return new ResponseMessage(false, "Invalid token");
            }

            var username = claimsPrincipal.Identity.Name;

            var user = await _userManager.FindByNameAsync(username);

            if (user == null || user.RefreshToken != tokenModel.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                throw new Exception("Invalid token");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var newAccessToken = await GenerateAcccessToken(user, userRoles.ToList());
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;

            await _userManager.UpdateAsync(user);

            return new ResponseMessage(true, "Refresh token successfully", new TokenModel
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        public async Task<string> GenerateAcccessToken(ApplicationUser user, IList<string> roles)
        {
            var issuer = _configuration["Jwt:ValidIssuer"];
            var audience = _configuration["Jwt:ValidAudience"];
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
            _ = int.TryParse(_configuration["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);

            var authClaims = new List<Claim>();

            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                }),
                Expires = DateTime.Now.AddMinutes(tokenValidityInMinutes),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials
                (new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256)
            };
            ((ClaimsIdentity)tokenDescriptor.Subject).AddClaims(authClaims);
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);
            var stringToken = tokenHandler.WriteToken(token);
            return stringToken;
        }

        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<ResponseMessage> RevokeToken(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return new ResponseMessage(false, "Invalid username");
            }

            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);

            return new ResponseMessage(true, "Revoke token successfully");
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false // Allows expired tokens
            };

            try
            {
                var claimsPrincipal = new JwtSecurityTokenHandler()
                    .ValidateToken(token, tokenValidationParameters, out var rawValidatedToken);

                if (!(rawValidatedToken is JwtSecurityToken validatedToken) || !validatedToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");

                return claimsPrincipal;
            }
            catch (Exception ex)
            {
                throw new SecurityTokenException("Invalid token", ex);
            }
        }

        public async Task<ResponseMessage> ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);

            if (user == null)
            {
                return new ResponseMessage(false, "Email not found");
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/api/authentication/resetpassword?token={resetToken}&email={user.Email}";
            var toEmail = new string[] { user.Email };
            var subject = "Password Reset";
            var body = $"Please click on the following link to reset your password: <a href='{resetLink}'>Reset Password</a>";

            var message = new MailMessage(toEmail, subject, body);

            await _mailService.SendEmailAsync(message);
            return new ResponseMessage(true, "Please check your mail!");
        }

        public async Task<ResponseMessage> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);

            if (user == null)
            {
                return new ResponseMessage(false, "Email not found");
            }

            var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.Password);

            if (!resetPassResult.Succeeded)
            {
                var errors = resetPassResult.Errors.Select(e => e.Description);
                return new ResponseMessage(false, string.Join(", ", errors));
            }

            return new ResponseMessage(true, "Password reset successful");
        }

        public async Task<ResponseMessage> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new ResponseMessage(false, "Confirm email failed");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                return new ResponseMessage(false, "Confirm email failed");
            }

            return new ResponseMessage(true, "Confirm email successful");
        }
    }
}