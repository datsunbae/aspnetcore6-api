using System.ComponentModel.DataAnnotations;
using api_aspnetcore6.Models;

namespace api_aspnetcore6.Dtos.Order
{
    public class OrderRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Status order is required")]
        [StatusOrderValidation]
        public string Status { get; set; }
        public bool IsPayment { get; set; }
    }

    public class StatusOrderValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && !IsValidStatusOrder(value.ToString()))
            {
                return new ValidationResult("Invalid status order value");
            }

            return ValidationResult.Success;
        }

        private bool IsValidStatusOrder(string status)
        {
            return status == StatusOrder.Open ||
                   status == StatusOrder.Confirmed ||
                   status == StatusOrder.Completed ||
                   status == StatusOrder.Canceled;
        }
    }
}