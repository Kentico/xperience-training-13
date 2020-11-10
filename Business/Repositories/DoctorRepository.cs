using System;
using System.Collections.Generic;

using CMS.Membership;

using XperienceAdapter.Repositories;
using XperienceAdapter.Services;
using Business.Models;
using CMS.DocumentEngine;
using Business.Extensions;
using System.Linq;

namespace Business.Repositories
{
    public class DoctorRepository : BasePageRepository<Doctor, CMS.DocumentEngine.Types.MedioClinic.Doctor>
    {
        private readonly IUserInfoProvider _userInfoProvider;

        public DoctorRepository(IRepositoryServices repositoryServices, IUserInfoProvider userInfoProvider) : base(repositoryServices)
        {
            _userInfoProvider = userInfoProvider ?? throw new ArgumentNullException(nameof(userInfoProvider));
        }

        public override Doctor MapDtoProperties(CMS.DocumentEngine.Types.MedioClinic.Doctor page, Doctor dto)
        {
            dto.UrlSlug = page.UrlSlug;
            dto.UserId = page.UserAccount;
            dto.UserName = _userInfoProvider.Get(page.UserAccount).UserName;
            dto.EmergencyShift = GetShiftDayOfWeek(page.Fields.EmergencyShift);
            dto.Degree = page.Degree;
            dto.Biography = page.Fields.Biography;
            dto.Specialty = page.Specialty;

            return dto;
        }

        private static DayOfWeek? GetShiftDayOfWeek(IEnumerable<TreeNode> dayOfWeekPage) =>
            dayOfWeekPage.ToDaysOfWeek().FirstOrDefault();
    }
}
