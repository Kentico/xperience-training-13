using System;
using System.Collections.Generic;
using System.Linq;

using CMS.Core;
using CMS.Helpers;
using CMS.MediaLibrary;
using CMS.MediaLibrary.Internal;
using CMS.UIControls;

public partial class CMSModules_MediaLibrary_Controls_MediaLibrary_MediaFileUsageDialog : CMSPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        EnsureScriptManager();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (QueryHelper.ValidateHash("hash"))
        {
            // Initialize controls
            SetupControls();
        }
        else
        {
            fileUsage.Visible = false;
            URLHelper.Redirect(AdministrationUrlHelper.GetErrorPageUrl("dialogs.badhashtitle", "dialogs.badhashtext"));
        }
    }


    private void SetupControls()
    {
        PageTitle.TitleText = GetString("medialibrary.dependencytracker.header");

        SetupUsages();
    }


    private void SetupUsages()
    {
        var usages = GetUsages();

        if (usages.Any())
        {
            ShowInformation(GetString("medialibrary.dependencytracker.infomessage"));
        }
        else
        {
            ShowInformation(GetString("medialibrary.dependencytracker.nodatafound"));
        }

        fileUsage.Setup(usages);
    }


    private IEnumerable<IMediaFileUsageSearchResult> GetUsages()
    {
        var fileId = QueryHelper.GetInteger("fileid", 0);
        if (fileId <= 0)
        {
            return Enumerable.Empty<IMediaFileUsageSearchResult>();
        }
        var fileInfo = MediaFileInfo.Provider.Get(fileId);

        if (fileInfo == null)
        {
            return Enumerable.Empty<IMediaFileUsageSearchResult>();
        }

        return Service.Resolve<IMediaFileUsageRetriever>().Get(fileInfo);
    }
}