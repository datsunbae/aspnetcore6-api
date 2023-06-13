using System.ComponentModel.DataAnnotations;

namespace api_aspnetcore6.Dtos.Category
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter a category name")]
        [MaxLength(255)]
        public string Name { get; set; }
    }
}