using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Core;

namespace Business.Models
{
    public class EmailViewModel
    {
        [Required(ErrorMessage = "General.RequireEmail")]
        [DisplayName("General.EmailAddress")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Models.EmailFormat")]
        [MaxLength(100, ErrorMessage = "Models.MaxLength")]
        public string? Email { get; set; }
    }
}