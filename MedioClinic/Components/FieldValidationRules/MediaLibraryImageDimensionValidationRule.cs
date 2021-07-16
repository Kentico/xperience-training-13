using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CMS.Helpers;
using CMS.MediaLibrary;
using CMS.SiteProvider;

using Kentico.Forms.Web.Mvc;

using XperienceAdapter.Repositories;

namespace MedioClinic.Components.FieldValidationRules
{
    [Serializable]
    public class MediaLibraryImageDimensionValidationRule : ValidationRule<string>
    {
        private const string MinimumWidthKey = ComponentIdentifiers.ImageValidationRule + ".MinimumWidth";
        private const string MaximumWidthKey = ComponentIdentifiers.ImageValidationRule + ".MaximumWidth";
        private const string MinimumHeightKey = ComponentIdentifiers.ImageValidationRule + ".MinimumHeight";
        private const string MaximumHeightKey = ComponentIdentifiers.ImageValidationRule + ".MaximumHeight";

        [EditingComponent(IntInputComponent.IDENTIFIER, Label = "{$" + MinimumWidthKey + "$}", Order = 0)]
        public int MinimumWidth { get; set; }

        [EditingComponent(IntInputComponent.IDENTIFIER, Label = "{$" + MaximumWidthKey + "$}", Order = 1)]
        public int MaximumWidth { get; set; }

        [EditingComponent(IntInputComponent.IDENTIFIER, Label = "{$" + MinimumHeightKey + "$}", Order = 2)]
        public int MinimumHeight { get; set; }

        [EditingComponent(IntInputComponent.IDENTIFIER, Label = "{$" + MaximumHeightKey + "$}", Order = 3)]
        public int MaximumHeight { get; set; }

        public override string GetTitle()
        {
            return $"{ResHelper.GetString(MinimumWidthKey)}: {MinimumWidth}. " +
                $"{ResHelper.GetString(MaximumWidthKey)}: {MaximumWidth}. " +
                $"{ResHelper.GetString(MinimumHeightKey)}: {MinimumHeight}. " +
                $"{ResHelper.GetString(MaximumHeightKey)}: {MaximumHeight}.";
        }

        protected override bool Validate(string value)
        {
            Guid guid;
            guid = Guid.TryParse(value, out guid) ? guid : Guid.Empty;

            if (guid != Guid.Empty)
            {
                var mediaFileInfo = MediaFileInfoProvider.GetMediaFileInfo(guid, SiteContext.CurrentSiteName);

                return mediaFileInfo != null
                    && MinimumWidth <= mediaFileInfo.FileImageWidth
                    && mediaFileInfo.FileImageWidth <= MaximumWidth
                    && MinimumHeight <= mediaFileInfo.FileImageHeight
                    && mediaFileInfo.FileImageHeight <= MaximumHeight;
            }

            return false;
        }
    }
}
