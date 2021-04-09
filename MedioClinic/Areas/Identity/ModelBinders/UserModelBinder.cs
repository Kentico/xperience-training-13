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

namespace MedioClinic.Areas.Identity.ModelBinders
{
    internal class UserModelBinder : IModelBinder
    {
        private readonly MedioClinicUserManager _medioClinicUserManager;

        private readonly Dictionary<Type, (ModelMetadata, IModelBinder)> _binders;

        public UserModelBinder(MedioClinicUserManager medioClinicUserManager, Dictionary<Type, (ModelMetadata, IModelBinder)> binders)
        {
            _medioClinicUserManager = medioClinicUserManager ?? throw new ArgumentNullException(nameof(medioClinicUserManager));
            _binders = binders ?? throw new ArgumentNullException(nameof(binders));
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var user = bindingContext.HttpContext.User;

            if (user != null)
            {
                var medioClinicUser = await _medioClinicUserManager.GetUserAsync(user);

                if (medioClinicUser != null)
                {
                    IModelBinder modelBinder;
                    ModelMetadata modelMetadata;
                    var roles = _medioClinicUserManager.GetRolesAsync(medioClinicUser).Result.ToMedioClinicRoles();

                    if (FlagEnums.HasAnyFlags(Roles.Doctor, roles))
                    {
                        (modelMetadata, modelBinder) = _binders[typeof(DoctorViewModel)];
                    }
                    else if (FlagEnums.HasAnyFlags(Roles.Patient, roles))
                    {
                        (modelMetadata, modelBinder) = _binders[typeof(PatientViewModel)];
                    }
                    else
                    {
                        bindingContext.Result = ModelBindingResult.Failed();

                        return;
                    }

                    var newBindingContext = DefaultModelBindingContext.CreateBindingContext(
                        bindingContext.ActionContext,
                        bindingContext.ValueProvider,
                        modelMetadata,
                        bindingInfo: null,
                        bindingContext.ModelName);

                    await modelBinder.BindModelAsync(newBindingContext);
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