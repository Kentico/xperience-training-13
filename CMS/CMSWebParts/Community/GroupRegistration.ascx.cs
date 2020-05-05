using System;

using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;

public partial class CMSWebParts_Community_GroupRegistration : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether form should be hidden after successful registration.
    /// </summary>
    public bool HideFormAfterRegistration
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideFormAfterRegistration"), false);
        }
        set
        {
            SetValue("HideFormAfterRegistration", value);
        }
    }


    /// <summary>
    /// Gets or sets text which should be displayed after successful registration.
    /// </summary>
    public string SuccessfullRegistrationText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SuccessfullRegistrationText"), groupRegistrationElem.SuccessfullRegistrationText);
        }
        set
        {
            SetValue("SuccessfullRegistrationText", value);
            groupRegistrationElem.SuccessfullRegistrationText = value;
        }
    }


    /// <summary>
    /// Emails of admins capable of approving the group.
    /// </summary>
    public string SendWaitingForApprovalEmailTo
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SendWaitingForApprovalEmailTo"), String.Empty);
        }
        set
        {
            SetValue("SendWaitingForApprovalEmailTo", value);
            groupRegistrationElem.SendWaitingForApprovalEmailTo = value;
        }
    }


    /// <summary>
    /// Gets or sets text which should be displayed after successful registration and waiting for approving.
    /// </summary>
    public string SuccessfullRegistrationWaitingForApprovalText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SuccessfullRegistrationWaitingForApprovalText"), groupRegistrationElem.SuccessfullRegistrationWaitingForApprovalText);
        }
        set
        {
            SetValue("SuccessfullRegistrationWaitingForApprovalText", value);
            groupRegistrationElem.SuccessfullRegistrationWaitingForApprovalText = value;
        }
    }


    /// <summary>
    /// If true, the group must be approved before it can be active.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), false);
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
        }
    }


    /// <summary>
    /// If true, the group must be approved before it can be active.
    /// </summary>
    public bool RequireApproval
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RequireApproval"), false);
        }
        set
        {
            SetValue("RequireApproval", value);
        }
    }


    /// <summary>
    /// Alias path of the document structure which will be copied as the group content.
    /// </summary>
    public string GroupTemplateSourceAliasPath
    {
        get
        {
            return ValidationHelper.GetString(GetValue("GroupTemplateSourceAliasPath"), "");
        }
        set
        {
            SetValue("GroupTemplateSourceAliasPath", value);
        }
    }


    /// <summary>
    /// Alias where the group content will be created by copying the source template.
    /// </summary>
    public string GroupTemplateTargetAliasPath
    {
        get
        {
            return ValidationHelper.GetString(GetValue("GroupTemplateTargetAliasPath"), "");
        }
        set
        {
            SetValue("GroupTemplateTargetAliasPath", value);
        }
    }


    /// <summary>
    /// Gets or sets the document url under which will be accessible the profile of newly created group.
    /// </summary>
    public string GroupProfileURLPath
    {
        get
        {
            return ValidationHelper.GetString(GetValue("GroupProfileURLPath"), "");
        }
        set
        {
            SetValue("GroupProfileURLPath", value);
        }
    }


    /// <summary>
    /// Gets or sets the url, where is user redirected after registration.
    /// </summary>
    public string RedirectToURL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RedirectToURL"), "");
        }
        set
        {
            SetValue("RedirectToURL", value);
        }
    }


    /// <summary>
    /// Gets or sets the label text of display name field.
    /// </summary>
    public string GroupNameLabelText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("GroupNameLabelText"), GetString("Groups.GroupName") + ResHelper.Colon);
        }
        set
        {
            SetValue("GroupNameLabelText", value);
            groupRegistrationElem.GroupNameLabelText = value;
        }
    }


    /// <summary>
    /// Indicates if group forum should be created.
    /// </summary>
    public bool CreateForum
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CreateForum"), true);
        }
        set
        {
            SetValue("CreateForum", value);
            groupRegistrationElem.CreateForum = value;
        }
    }


    /// <summary>
    /// Indicates if group media library should be created.
    /// </summary>
    public bool CreateMediaLibrary
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CreateMediaLibrary"), true);
        }
        set
        {
            SetValue("CreateMediaLibrary", value);
            groupRegistrationElem.CreateMediaLibrary = value;
        }
    }


    /// <summary>
    /// Indicates if search indexes should be created.
    /// </summary>
    public bool CreateSearchIndexes
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CreateSearchIndexes"), true);
        }
        set
        {
            SetValue("CreateSearchIndexes", value);
            groupRegistrationElem.CreateSearchIndexes = value;
        }
    }

    #endregion


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            if (SiteContext.CurrentSite != null)
            {
                groupRegistrationElem.SiteID = SiteContext.CurrentSite.SiteID;
            }

            groupRegistrationElem.HideFormAfterRegistration = HideFormAfterRegistration;
            groupRegistrationElem.SuccessfullRegistrationText = SuccessfullRegistrationText;
            groupRegistrationElem.SuccessfullRegistrationWaitingForApprovalText = SuccessfullRegistrationWaitingForApprovalText;
            groupRegistrationElem.GroupNameLabelText = GroupNameLabelText;
            groupRegistrationElem.CombineWithDefaultCulture = CombineWithDefaultCulture;
            groupRegistrationElem.RequireApproval = RequireApproval;
            groupRegistrationElem.GroupTemplateSourceAliasPath = GroupTemplateSourceAliasPath;
            groupRegistrationElem.GroupTemplateTargetAliasPath = GroupTemplateTargetAliasPath;
            groupRegistrationElem.GroupProfileURLPath = GroupProfileURLPath;
            groupRegistrationElem.RedirectToURL = RedirectToURL;
            groupRegistrationElem.SendWaitingForApprovalEmailTo = SendWaitingForApprovalEmailTo;
            groupRegistrationElem.CreateForum = CreateForum;
            groupRegistrationElem.CreateMediaLibrary = CreateMediaLibrary;
            groupRegistrationElem.CreateSearchIndexes = CreateSearchIndexes;
            groupRegistrationElem.IsLiveSite = true;
        }
    }
}