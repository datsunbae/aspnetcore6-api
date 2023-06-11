using api_aspnetcore6.Dtos;

namespace api_aspnetcore6.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<ResponseMessage> Login(LoginDTO loginDTO);
        Task<ResponseMessage> Register(RegisterDTO registerDTO);
        Task<ResponseMessage> RefreshToken(TokenModel tokenModel);
        Task<ResponseMessage> RevokeToken(string username);
        Task<ResponseMessage> ForgotPassword(ForgotPasswordModel forgotPasswordModel);
        Task<ResponseMessage> ResetPassword(ResetPasswordModel resetPasswordModel);
        Task<ResponseMessage> ConfirmEmail(string token, string email);
    }
}