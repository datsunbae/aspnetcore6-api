using System.ComponentModel.DataAnnotations;

namespace api_aspnetcore6.Dtos.User
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Please enter a username")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please enter a password")]
        public string Password { get; set; }
    }
}