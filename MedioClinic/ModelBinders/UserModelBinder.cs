using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using EnumsNET;

using Identity;
using Identity.Extensions;
using Identity.Models;
using Identity.Models.Profile;

namespace MedioClinic.ModelBinders
{
    internal class UserModelBinder : IModelBinder
    {
        private readonly IModelMetadataProvider _modelMetadataProvider;

        private readonly MedioClinicUserManager _medioClinicUserManager;

        public UserModelBinder(IModelMetadataProvider modelMetadataProvider, MedioClinicUserManager medioClinicUserManager)
        {
            _modelMetadataProvider = modelMetadataProvider ?? throw new ArgumentNullException(nameof(modelMetadataProvider));
            _medioClinicUserManager = medioClinicUserManager ?? throw new ArgumentNullException(nameof(medioClinicUserManager));
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var user = bindingContext.HttpContext.User;

            if (user != null)
            {
                var medioClinicUser = await _medioClinicUserManager.GetUserAsync(user);

                if (medioClinicUser != null)
                {
                    Type modelType;
                    var roles = _medioClinicUserManager.GetRolesAsync(medioClinicUser).Result.ToMedioClinicRoles();

                    if (FlagEnums.HasAnyFlags(Roles.Doctor, roles))
                    {
                        modelType = typeof(DoctorViewModel);
                    }
                    else if (FlagEnums.HasAnyFlags(Roles.Patient, roles))
                    {
                        modelType = typeof(PatientViewModel);
                    }
                    else
                    {
                        bindingContext.Result = ModelBindingResult.Failed();

                        return;
                    }

                    var modelMetadata = _modelMetadataProvider.GetMetadataForType(modelType);

                    var newBindingContext = DefaultModelBindingContext.CreateBindingContext(
                        bindingContext.ActionContext,
                        bindingContext.ValueProvider,
                        modelMetadata,
                        bindingInfo: null,
                        bindingContext.ModelName);

                    await BindModelAsync(newBindingContext);
                    bindingContext.Result = newBindingContext.Result;

                    if (newBindingContext.Result.IsModelSet)
                    {
                        bindingContext.ValidationState[newBindingContext.Result] = new ValidationStateEntry
                        {
                            Metadata = modelMetadata,
                        };
                    }
                }
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Failed();

                return;
            }
        }
    }
}