using System;
using System.Collections.Generic;
using System.Linq;

using Kentico.Forms.Web.Mvc;
using Kentico.Web.Mvc;

using Business.Repositories;

namespace MedioClinic.Models.FormComponents
{
    public class AirportSelectionComponent : DropDownComponent
    {
        private readonly IAirportRepository _airportRepository;

        public AirportSelectionComponent(IAirportRepository airportRepository)
        {
            _airportRepository = airportRepository ?? throw new ArgumentNullException(nameof(airportRepository));
        }

        protected override IEnumerable<HtmlOptionItem> GetHtmlOptions() =>
            _airportRepository
                .GetAll()
                .Select(dto => new HtmlOptionItem
                {
                    Text = dto.AirportName,
                    Value = dto.AirportIataCode
                });
    }
}