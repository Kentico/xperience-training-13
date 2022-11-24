using Microsoft.AspNetCore.Mvc;

using Kentico.PageBuilder.Web.Mvc.Personalization;

using MedioClinic.Personalization;

namespace MedioClinic.Controllers
{
    public class ComesFromBigUsCityController : ConditionTypeController<ComesFromBigUsCityConditionType>
    {
        [HttpPost]
        public ActionResult Index()
        {
            var conditionTypeParameters = GetParameters();

            var model = new ComesFromBigUsCityViewModel
            {
                IsForBigUsCity = conditionTypeParameters.IsForBigCities
            };

            return ConfigurationDialog(model);
        }

        [HttpPost]
        public ActionResult Validate(ComesFromBigUsCityViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var model = new ComesFromBigUsCityViewModel
                {
                    IsForBigUsCity = viewModel.IsForBigUsCity
                };

                return ConfigurationDialog(model);
            }

            var parameters = new ComesFromBigUsCityConditionType
            {
                IsForBigCities = viewModel.IsForBigUsCity
            };

            return new ConditionTypeValidationResult(parameters);
        }

        private ActionResult ConfigurationDialog(ComesFromBigUsCityViewModel model) =>
            PartialView(
                "~/Components/Personalization/ConditionTypes/ComesFromBigUsCity/_ComesFromBigUsCityConfiguration.cshtml", 
                model);
    }
}
