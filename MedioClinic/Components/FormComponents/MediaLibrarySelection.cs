using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CMS.Base;
using CMS.MediaLibrary;

using Kentico.Forms.Web.Mvc;
using Kentico.Web.Mvc;

namespace MedioClinic.Components.FormComponents
{
    public class MediaLibrarySelection : DropDownComponent
    {
        private readonly IMediaLibraryInfoProvider _mediaLibraryInfoProvider;

        private readonly ISiteService _siteService;

        public MediaLibrarySelection(IMediaLibraryInfoProvider mediaLibraryInfoProvider, ISiteService siteService)
        {
            _mediaLibraryInfoProvider = mediaLibraryInfoProvider ?? throw new ArgumentNullException(nameof(mediaLibraryInfoProvider));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
        }

        protected override IEnumerable<HtmlOptionItem> GetHtmlOptions() =>
            _mediaLibraryInfoProvider
                .Get()
                .WhereEquals("LibrarySiteID", _siteService.CurrentSite.SiteID)
                .TypedResult
                .Items
                .Select(libraryInfo => new HtmlOptionItem
                {
                    Text = libraryInfo.LibraryName,
                    Value = libraryInfo.LibraryID.ToString()
                });
    }
}
