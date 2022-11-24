using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

using CMS.Personas;
using Kentico.PageBuilder.Web.Mvc.Personalization;

using MedioClinic.Personalization;

namespace MedioClinic.Controllers
{
    public class IsInPersonaController : ConditionTypeController<IsInPersonaConditionType>
    {
        private readonly IPersonaInfoProvider _personaInfoProvider;
        private readonly IPersonaPictureUrlCreator _pictureUrlCreator;

        public IsInPersonaController(IPersonaInfoProvider personaInfoProvider, IPersonaPictureUrlCreator pictureUrlCreator)
        {
            _personaInfoProvider = personaInfoProvider ?? throw new ArgumentNullException(nameof(personaInfoProvider));
            _pictureUrlCreator = pictureUrlCreator ?? throw new ArgumentNullException(nameof(pictureUrlCreator));
        }

        [HttpPost]
        public ActionResult Index()
        {
            var conditionTypeParameters = GetParameters();
   
            var viewModel = new IsInPersonaViewModel
            {
                PersonaCodeName = conditionTypeParameters.PersonaCodeName,
                AllPersonas = GetAllPersonasViewModel(conditionTypeParameters.PersonaCodeName!)
            };

            return PartialView("~/Components/Personalization/ConditionTypes/IsInPersona/_IsInPersonaConfiguration.cshtml", viewModel);
        }

        [HttpPost]
        public ActionResult Validate(IsInPersonaViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.AllPersonas = GetAllPersonasViewModel();

                return PartialView("~/Components/Personalization/ConditionTypes/IsInPersona/_IsInPersonaConfiguration.cshtml", viewModel);
            }

            var parameters = new IsInPersonaConditionType
            {
                PersonaCodeName = viewModel.PersonaCodeName!
            };

            return new ConditionTypeValidationResult(parameters);
        }

        private List<IsInPersonaListItemViewModel> GetAllPersonasViewModel(string selectedPersonaName = "")
        {
            var allPersonas = GetAllPersonas()
                .Select(persona => new IsInPersonaListItemViewModel
                    {
                        CodeName = persona.PersonaName,
                        DisplayName = persona.PersonaDisplayName,
                        ImagePath = _pictureUrlCreator.CreatePersonaPictureUrl(persona, 50),
                        Selected = persona.PersonaName == selectedPersonaName
                    })
                .ToList();

            if (allPersonas.Count > 0 && !allPersonas.Exists(x => x.Selected))
            {
                allPersonas.First().Selected = true;
            }

            return allPersonas;
        }

        private IEnumerable<PersonaInfo> GetAllPersonas() => _personaInfoProvider.Get();
    }
}