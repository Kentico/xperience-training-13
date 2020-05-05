using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.EmailEngine;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;


[Title("general.attachments")]
public partial class CMSModules_AdminControls_Controls_MetaFiles_MetaFileDialog : CMSModalPage
{
    #region "Properties"

    protected int objectId = -1;
    protected string mObjectType;
    protected string mObjectCategory;


    /// <summary>
    /// Object ID
    /// </summary>
    protected int ObjectId
    {
        get
        {
            if (objectId < 0)
            {
                objectId = QueryHelper.GetInteger("objectid", 0);
            }
            return objectId;
        }
    }


    /// <summary>
    /// Metafile object type
    /// </summary>
    protected string ObjectType
    {
        get
        {
            if (string.IsNullOrEmpty(mObjectType))
            {
                mObjectType = QueryHelper.GetString("objecttype", string.Empty);
            }
            return mObjectType;
        }
    }


    /// <summary>
    /// Metafile category
    /// </summary>
    protected string ObjectCategory
    {
        get
        {
            if (string.IsNullOrEmpty(mObjectCategory))
            {
                mObjectCategory = QueryHelper.GetString("category", string.Empty);
            }
            return mObjectCategory;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Init(object sender, EventArgs e)
    {
        var user = MembershipContext.AuthenticatedUser;

        // Check the license and site availability for Newsletters
        switch (ObjectType)
        {
            case PredefinedObjectType.NEWSLETTERISSUE:
            case PredefinedObjectType.NEWSLETTERISSUEVARIANT:
            case PredefinedObjectType.NEWSLETTERTEMPLATE:
                {
                    // Check the license
                    if (!string.IsNullOrEmpty(DataHelper.GetNotEmpty(RequestContext.CurrentDomain, string.Empty)))
                    {
                        LicenseHelper.CheckFeatureAndRedirect(RequestContext.CurrentDomain, FeatureEnum.Newsletters);
                    }

                    // Check site availability
                    if (!ResourceSiteInfoProvider.IsResourceOnSite("CMS.Newsletter", SiteContext.CurrentSiteName))
                    {
                        RedirectToResourceNotAvailableOnSite("CMS.Newsletter");
                    }

                    // Check UI permissions for CMS Desk -> Tools -> Newsletter
                    if (!user.IsAuthorizedPerUIElement("CMS.Newsletter", "Newsletter"))
                    {
                        RedirectToUIElementAccessDenied("CMS.Newsletter", "Newsletter");
                    }
                }
                break;
        }

        // Check permissions
        switch (ObjectType)
        {
            case PredefinedObjectType.NEWSLETTERISSUE:
            case PredefinedObjectType.NEWSLETTERISSUEVARIANT:
                // Check 'AuthorIssues' permission
                if (!user.IsAuthorizedPerResource("CMS.Newsletter", "AuthorIssues"))
                {
                    RedirectToAccessDenied("CMS.Newsletter", "AuthorIssues");
                }
                break;

            case PredefinedObjectType.NEWSLETTERTEMPLATE:
                // Check 'Managetemplates' permission
                if (!user.IsAuthorizedPerResource("CMS.Newsletter", "managetemplates"))
                {
                    RedirectToAccessDenied("CMS.Newsletter", "managetemplates");
                }
                break;

            case PredefinedObjectType.BIZFORM:
                // Check 'EditForm' permission
                if (!user.IsAuthorizedPerResource("cms.form", "EditForm"))
                {
                    RedirectToAccessDenied("cms.form", "EditForm");
                }
                break;

            case EmailTemplateInfo.OBJECT_TYPE:
                // Check "Modify" permission
                if (!user.IsAuthorizedPerResource("CMS.EmailTemplates", "Modify"))
                {
                    RedirectToAccessDenied("CMS.EmailTemplates", "Modify");
                }
                break;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (QueryHelper.ValidateHash("hash", "category"))
        {
            // Get site ID, if 0 then global object files are required
            int siteId = QueryHelper.GetInteger("siteid", -1);

            if (siteId < 0)
            {
                // If not set use current site ID
                siteId = SiteContext.CurrentSiteID;
            }

            // Initialize attachment list
            AttachmentList.Visible = true;
            AttachmentList.ObjectID = ObjectId;

            if (siteId > 0)
            {
                AttachmentList.SiteID = siteId;
            }

            AttachmentList.AllowPasteAttachments = QueryHelper.GetBoolean("allowpaste", true);
            AttachmentList.UseVirtualPathOnPaste = QueryHelper.GetBoolean("pasteVirtualPath", false);
            AttachmentList.ObjectType = ObjectType;
            AttachmentList.Category = ObjectCategory;
            AttachmentList.ItemsPerPage = 10;
            AttachmentList.OnAfterDelete += AttachmentList_OnAfterChange;
            AttachmentList.OnAfterUpload += AttachmentList_OnAfterChange;
            AttachmentList.IsLiveSite = false;
            AttachmentList.UploadMode = MultifileUploaderModeEnum.DirectMultiple;
            AttachmentList.Enabled = QueryHelper.GetBoolean("allowedit", true);
            AttachmentList.HideObjectMenu = QueryHelper.GetBoolean("hideobjectmenu", false);
        }
        else
        {
            AttachmentList.StopProcessing = true;
            AttachmentList.Visible = false;

            // Redirect to error page
            string url = ResolveUrl(AdministrationUrlHelper.GetErrorPageUrl("dialogs.badhashtitle", "dialogs.badhashtext", true));
            ScriptHelper.RegisterStartupScript(this, typeof(string), "BadHashRedirection", string.Format("window.location = '{0}';", url), true);
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (!AttachmentList.StopProcessing)
        {
            // Register 'paste' script
            string script = @"
var updateAttachmentCount = function () {};

cmsrequire(['CMS/EventHub'], function (EventHub) {
    updateAttachmentCount = function () {
        var hdnCount = document.getElementById('" + hdnCount.ClientID + @"').value;
        if (hdnCount)
        {
            EventHub.publish({ name: 'UpdateAttachmentsCount', onlySubscribed: true }, hdnCount);
        }
    };
});

function PasteImage(imageurl) {
    updateAttachmentCount();    
    if((wopener!=null)&&(wopener.PasteImage)){wopener.PasteImage(imageurl);}
    return CloseDialog();
}
";
            ScriptHelper.RegisterRequireJs(this);
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "PasteImageScript", script, true);

            SetCloseJavascript("updateAttachmentCount()");
        }
    }

    #endregion


    #region "Other events"

    protected void AttachmentList_OnAfterChange(object sender, EventArgs e)
    {
        // Get number of attachments
        InfoDataSet<MetaFileInfo> ds = MetaFileInfoProvider.GetMetaFiles(ObjectId, ObjectType, ObjectCategory, null, null, "MetafileID", -1);

        // Register script to update hdnCount value (it is used to update attachment count in wopener)
        ScriptHelper.RegisterStartupScript(this, typeof(string), "UpdateCountHolder", "document.getElementById('" + hdnCount.ClientID + "').value=" + ds.Items.Count.ToString(), true);
    }

    #endregion
}
