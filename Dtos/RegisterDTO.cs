using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace api_aspnetcore6.Dtos
{
    public class RegisterDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter user name")]
        [MaxLength(50)]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please enter password")]
        [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm Password is required")]
        [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Confirm Email is required")]
        [DefaultValue("User")]
        public string Role { get; set; }
    }
}