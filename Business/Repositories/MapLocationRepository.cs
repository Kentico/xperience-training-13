using System;
using System.Collections.Generic;
using System.Text;

using XperienceAdapter.Repositories;
using XperienceAdapter.Services;

namespace Business.Repositories
{
    public class MapLocationRepository : BasePageRepository<Models.MapLocation, CMS.DocumentEngine.Types.MedioClinic.MapLocation>
    {
        public MapLocationRepository(IRepositoryServices repositoryServices) : base(repositoryServices)
        {
        }

        public override void MapDtoProperties(CMS.DocumentEngine.Types.MedioClinic.MapLocation page, Models.MapLocation dto)
        {
            dto.Latitude = page.Latitude;
            dto.Longitude = page.Longitude;
        }
    }
}
