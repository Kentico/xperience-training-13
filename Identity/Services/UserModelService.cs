using System;
using System.Collections.Generic;
using System.Linq;

using Identity.Models;

namespace Identity.Services
{
    public class UserModelService : IUserModelService
    {
        public object MapToCustomModel(
            MedioClinicUser user,
            Type targetModelType,
            Dictionary<(string propertyName, Type propertyType), object>? customMappings = default)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (targetModelType == null)
            {
                throw new ArgumentNullException(nameof(targetModelType));
            }

            var userProperties = user.GetType().GetProperties();
            var targetProperties = targetModelType.GetProperties();
            var viewModel = Activator.CreateInstance(targetModelType);

            foreach (var targetProperty in targetProperties)
            {
                var propertyToMatch = (propertyName: targetProperty.Name, propertyType: targetProperty.PropertyType);

                var sourceProperty = userProperties.FirstOrDefault(
                    prop => prop.Name.Equals(targetProperty.Name, StringComparison.OrdinalIgnoreCase)
                    && prop.PropertyType == targetProperty.PropertyType
                    && targetProperty.CanWrite);

                if (customMappings != null && customMappings.Keys.Contains(propertyToMatch))
                {
                    targetProperty.SetValue(viewModel, customMappings[propertyToMatch]);
                }
                else if (sourceProperty != null)
                {
                    targetProperty.SetValue(viewModel, sourceProperty.GetValue(user));
                }
            }

            return viewModel!;
        }

        public MedioClinicUser MapToMedioClinicUser(
            object customModel,
            MedioClinicUser userToMapTo,
            Dictionary<(string propertyName, Type propertyType), object>? customMappings = default)
        {
            if (customModel == null)
            {
                throw new ArgumentNullException(nameof(customModel));
            }

            if (userToMapTo == null)
            {
                throw new ArgumentNullException(nameof(userToMapTo));
            }

            var customModelProperties = customModel.GetType().GetProperties();
            var userProperties = userToMapTo.GetType().GetProperties();

            foreach (var userProperty in userProperties)
            {
                var propertyToMatch = (propertyName: userProperty.Name, propertyType: userProperty.PropertyType);

                var sourceProperty = customModelProperties.FirstOrDefault(prop =>
                    prop.Name.Equals(userProperty.Name, StringComparison.OrdinalIgnoreCase)
                    && prop.PropertyType == userProperty.PropertyType);

                if (userProperty.CanWrite)
                {
                    if (customMappings != null && customMappings.Keys.Contains(propertyToMatch))
                    {
                        userProperty.SetValue(userToMapTo, customMappings[propertyToMatch]);
                    }
                    else if (sourceProperty != null)
                    {
                        userProperty.SetValue(userToMapTo, sourceProperty.GetValue(customModel));
                    } 
                }
            }

            return userToMapTo;
        }
    }
}
