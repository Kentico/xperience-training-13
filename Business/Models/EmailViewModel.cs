using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class EmailViewModel
    {
        [Required(ErrorMessage = "General.RequireEmail")]
        [Display(Name = "General.EmailAddress")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Models.EmailFormat")]
        [MaxLength(100, ErrorMessage = "Models.MaxLength")]
        public string? Email { get; set; }
    }
}