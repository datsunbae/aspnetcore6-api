using System.ComponentModel.DataAnnotations;

namespace api_aspnetcore6.Dtos.User
{
    public class TokenModel
    {
        [Required(ErrorMessage = "Please enter a access token")]
        public string AccessToken { get; set; }
        [Required(ErrorMessage = "Please enter a refresh token token")]
        public string RefreshToken { get; set; }
    }
}