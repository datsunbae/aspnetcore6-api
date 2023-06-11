using System.ComponentModel.DataAnnotations;

namespace api_aspnetcore6.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter a category name")]
        [MaxLength(255)]
        public string Name { get; set; }
        public virtual ICollection<Product>? Products { get; set; }
    }
}