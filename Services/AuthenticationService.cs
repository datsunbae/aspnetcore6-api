using api_aspnetcore6.Repositories.Interfaces;
using api_aspnetcore6.Dtos;
using api_aspnetcore6.Services.Interfaces;

namespace api_aspnetcore6.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationRepository _authenticationRepository;
        public AuthenticationService(IAuthenticationRepository authenticationRepository)
        {
            _authenticationRepository = authenticationRepository;
        }

        public Task<ResponseMessage> Login(LoginDTO loginDTO)
        {
            return _authenticationRepository.Login(loginDTO);
        }

        public async Task<ResponseMessage> Register(RegisterDTO registerDTO)
        {
            return await _authenticationRepository.Register(registerDTO);
        }

        public async Task<ResponseMessage> RefreshToken(TokenModel tokenModel)
        {
            return await _authenticationRepository.RefreshToken(tokenModel);
        }

        public async Task<ResponseMessage> RevokeToken(string username)
        {
            return await _authenticationRepository.RevokeToken(username);
        }

        public async Task<ResponseMessage> ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            return await _authenticationRepository.ForgotPassword(forgotPasswordModel);
        }

        public async Task<ResponseMessage> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            return await _authenticationRepository.ResetPassword(resetPasswordModel);
        }

        public async Task<ResponseMessage> ConfirmEmail(string token, string email)
        {
            return await _authenticationRepository.ConfirmEmail(token, email);
        }
    }
}