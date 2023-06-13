using System.ComponentModel.DataAnnotations;

namespace api_aspnetcore6.Dtos
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}