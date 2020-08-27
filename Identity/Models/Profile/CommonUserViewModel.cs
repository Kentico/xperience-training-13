using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

//using Business.Attributes;
using Business.Models;
using Identity.Models.Account;

namespace Identity.Models.Profile
{
    public class CommonUserViewModel
    {
        [Display(Name = "Models.Profile.CommonUserViewModel.Id")]
        public int Id { get; set; }

        [HiddenInput]
        public string? UserName { get; set; }

        [Required]
        [Display(Name = "Models.Profile.CommonUserViewModel.FirstName")]
        [MaxLength(100, ErrorMessage = "Models.MaxLength")]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Models.Profile.CommonUserViewModel.LastName")]
        [MaxLength(100, ErrorMessage = "Models.MaxLength")]
        public string? LastName { get; set; }

        [Display(Name = "Models.Profile.CommonUserViewModel.FullName")]
        public string FullName => $"{FirstName} {LastName}";

        [Display(Name = "Models.Profile.CommonUserViewModel.DateOfBirth")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        [Display(Name = "Models.Profile.CommonUserViewModel.City")]
        public string? City { get; set; }

        [Display(Name = "Models.Profile.CommonUserViewModel.Street")]
        public string? Street { get; set; }

        public EmailViewModel EmailViewModel { get; set; } = new EmailViewModel();

        [DataType(DataType.PhoneNumber)]
        [Phone(ErrorMessage = "Models.PhoneFormat")]
        public string? Phone { get; set; }

        [Display(Name = "Models.Profile.CommonUserViewModel.Nationality")]
        public string? Nationality { get; set; }

        [Display(Name = "Models.Profile.CommonUserViewModel.AvatarFile")]
        [FileExtensions(ErrorMessage = "Models.AllowedExtensions")]
        public IFormFile? AvatarFile { get; set; }
    }
}