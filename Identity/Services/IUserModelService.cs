using System;
using System.Collections.Generic;

using Core;
using Identity.Models;

namespace Identity.Services
{
    /// <summary>
    /// Custom mapper between Identity user models and custom user models.
    /// </summary>
    public interface IUserModelService : IService
    {
        /// <summary>
        /// Maps properties of a <see cref="MedioClinicUser"/> object to properties with the same name and type in the <paramref name="targetModelType"/> object.
        /// </summary>
        /// <param name="user">The source user object.</param>
        /// <param name="targetModelType">The type of the output object.</param>
        /// <param name="customMappings">Custom mappings of properties with different names and/or types.</param>
        /// <returns>The <paramref name="targetModelType"/> object with mapped properties.</returns>
        object MapToCustomModel(MedioClinicUser user, Type targetModelType, Dictionary<(string propertyName, Type propertyType), object>? customMappings = default);

        /// <summary>
        /// Maps properties of the <paramref name="customModel"/> object to properties of the same name and type in the <see cref="MedioClinicUser"/> object.
        /// </summary>
        /// <param name="customModel">The source model object.</param>
        /// <param name="userToMapTo">The target Identity user object.</param>
        /// <param name="customMappings">Custom mappings of properties with different names and/or types.</param>
        /// <returns>The mapped <see cref="MedioClinicUser"/> object.</returns>
        MedioClinicUser MapToMedioClinicUser(object customModel, MedioClinicUser userToMapTo, Dictionary<(string propertyName, Type propertyType), object>? customMappings = default);
    }
}
