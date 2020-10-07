using Microsoft.Extensions.Localization;
using System;

namespace XperienceAdapter.Localization
{
    public class XperienceStringLocalizerFactory : IStringLocalizerFactory
    {
        public IStringLocalizer Create(Type resourceSource)
        {
            return new XperienceStringLocalizer();
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new XperienceStringLocalizer();
        }
    }
}