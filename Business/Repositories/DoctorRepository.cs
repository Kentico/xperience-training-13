using System;
using System.Collections.Generic;
using System.Text;

using CMS.Membership;

using XperienceAdapter.Repositories;
using XperienceAdapter.Services;
using Business.Models;

namespace Business.Repositories
{
    public class DoctorRepository : BasePageRepository<Doctor, CMS.DocumentEngine.Types.MedioClinic.Doctor>
    {
        private readonly IUserInfoProvider _userInfoProvider;

        private readonly IAvatarService _avatarService;

        public DoctorRepository(IRepositoryServices repositoryServices, IUserInfoProvider userInfoProvider, IAvatarService avatarService) : base(repositoryServices)
        {
            _userInfoProvider = userInfoProvider ?? throw new ArgumentNullException(nameof(userInfoProvider));
            _avatarService = avatarService ?? throw new ArgumentNullException(nameof(avatarService));
        }

        public override Doctor MapDtoProperties(CMS.DocumentEngine.Types.MedioClinic.Doctor page, Doctor dto)
        {
            dto.UserName = _userInfoProvider.Get(page.UserAccount).UserName;

            return dto;
        }
    }
}
