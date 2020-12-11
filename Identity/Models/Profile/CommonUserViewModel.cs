using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Business.Models;

namespace Identity.Models.Profile
{
    public class CommonUserViewModel
    {
        [Display(Name = "Identity.Profile.CommonUserViewModel.Id")]
        public int Id { get; set; }

        [HiddenInput]
        public string? UserName { get; set; }

        [Required]
        [Display(Name = "General.FirstName")]
        [MaxLength(100, ErrorMessage = "Models.MaxLength")]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "General.LastName")]
        [MaxLength(100, ErrorMessage = "Models.MaxLength")]
        public string? LastName { get; set; }

        [Display(Name = "General.FullName")]
        public string FullName => $"{FirstName} {LastName}";

        [Display(Name = "Identity.DateOfBirth")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        [Display(Name = "General.City")]
        public string? City { get; set; }

        [Display(Name = "General.Street")]
        public string? Street { get; set; }

        public EmailViewModel EmailViewModel { get; set; } = new EmailViewModel();

        /// <summary>
        /// Use the <see cref="DataTypeAttribute"/> and <see cref="PhoneAttribute"/> attributes
        /// if you don't plan on using the placeholder in the HTML input fields.
        /// </summary>
        [Display(Name = "General.Phone")]
        //[DataType(DataType.PhoneNumber)]
        //[Phone(ErrorMessage = "Models.PhoneFormat")]
        public string? Phone { get; set; }

        [Display(Name = "Identity.Nationality")]
        [UIHint("Country")]
        public string? Nationality { get; set; }

        [Display(Name = "Identity.Profile.AvatarFile")]
        public IFormFile? AvatarFile { get; set; }
    }
}