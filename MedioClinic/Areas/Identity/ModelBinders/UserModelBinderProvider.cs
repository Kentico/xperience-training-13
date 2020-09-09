using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

using Identity.Models.Profile;

namespace MedioClinic.Areas.Identity.ModelBinders
{
    public class UserModelBinderProvider : IModelBinderProvider
    {
        // TODO: Nullable return value?
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType != typeof(IUserViewModel))
            {
                return null!;
            }

            return new BinderTypeModelBinder(typeof(UserModelBinder));
        }
    }
}
