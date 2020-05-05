using System;

using CMS.Base;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.MacroEngine;
using CMS.PortalEngine.Web.UI;
using CMS.DocumentEngine;
using CMS.SiteProvider;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;
using CMS.Membership;
using CMS.PortalEngine;

public partial class CMSWebParts_UserContributions_EditContribution : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether deleting document is allowed.
    /// </summary>
    public bool AllowDelete
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowDelete"), editForm.AllowDelete);
        }
        set
        {
            SetValue("AllowDelete", value);
            editForm.AllowDelete = value;
        }
    }


    /// <summary>
    /// Gets or sets group of users which can work with the documents.
    /// </summary>
    public UserContributionAllowUserEnum AllowUsers
    {
        get
        {
            return (UserContributionAllowUserEnum)(ValidationHelper.GetInteger(GetValue("AllowUsers"), 2));
        }
        set
        {
            SetValue("AllowUsers", Convert.ToInt32(value));
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the permissions are checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), true);
        }
        set
        {
            SetValue("CheckPermissions", value);
            editForm.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Gets or sets alternative form name.
    /// </summary>
    public string AlternativeFormName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AlternativeFormName"), editForm.AlternativeFormName);
        }
        set
        {
            SetValue("AlternativeFormName", value);
        }
    }


    /// <summary>
    /// Gets or sets the message which is displayed after validation failed.
    /// </summary>
    public string ValidationErrorMessage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ValidationErrorMessage"), editForm.ValidationErrorMessage);
        }
        set
        {
            SetValue("ValidationErrorMessage", value);
        }
    }


    /// <summary>
    /// Gets or sets edit button label.
    /// </summary>
    public string EditButtonText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("EditButtonText"), "general.edit");
        }
        set
        {
            SetValue("EditButtonText", value);
            btnEdit.ResourceString = value;
        }
    }


    /// <summary>
    /// Gets or sets delete button label.
    /// </summary>
    public string DeleteButtonText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("DeleteButtonText"), "general.delete");
        }
        set
        {
            SetValue("DeleteButtonText", value);
            btnDelete.ResourceString = value;
        }
    }


    /// <summary>
    /// Gets or sets close edit mode button label.
    /// </summary>
    public string CloseEditModeButtonText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("CloseEditModeButtonText"), "EditContribution.CloseButton");
        }
        set
        {
            SetValue("CloseEditModeButtonText", value);
        }
    }


    /// <summary>
    /// Gets or sets value that indicates whether logging activity is performed.
    /// </summary>
    public bool LogActivity
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("LogActivity"), false);
        }
        set
        {
            SetValue("LogActivity", value);
            editForm.LogActivity = value;
        }
    }

    #endregion


    #region "Document properties"

    /// <summary>
    /// Gets or sets the culture version of the displayed content.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("CultureCode"), ""), LocalizationContext.PreferredCultureCode);
        }
        set
        {
            SetValue("CultureCode", value);
        }
    }


    /// <summary>
    /// Gets or sets the path to the document.
    /// </summary>
    public string Path
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Path"), DocumentContext.CurrentAliasPath);
        }
        set
        {
            SetValue("Path", value);
        }
    }


    /// <summary>
    /// Gets or sets the codename of the site from which you want to display the content.
    /// </summary>
    public string SiteName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("SiteName"), ""), SiteContext.CurrentSiteName);
        }
        set
        {
            SetValue("SiteName", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            editForm.StopProcessing = true;
            editForm.Visible = false;
        }
        else
        {
            pnlEdit.Visible = false;

            var currentUser = MembershipContext.AuthenticatedUser;

            // Get the document
            var node = GetDocument(currentUser);
            if (node != null)
            {
                bool authorized = false;

                // Check user group
                switch (AllowUsers)
                {
                    case UserContributionAllowUserEnum.All:
                        authorized = true;
                        break;

                    case UserContributionAllowUserEnum.Authenticated:
                        authorized = AuthenticationHelper.IsAuthenticated();
                        break;

                    case UserContributionAllowUserEnum.DocumentOwner:
                        authorized = (node.NodeOwner == currentUser.UserID);
                        break;
                }

                bool authorizedDelete = authorized;

                // Check control access permissions
                if (authorized && CheckPermissions)
                {
                    // Node owner has always permissions
                    if (node.NodeOwner != currentUser.UserID)
                    {
                        authorized &= (currentUser.IsAuthorizedPerDocument(node, new[] { NodePermissionsEnum.Read, NodePermissionsEnum.Modify }) == AuthorizationResultEnum.Allowed);
                        authorizedDelete &= (currentUser.IsAuthorizedPerDocument(node, new[] { NodePermissionsEnum.Read, NodePermissionsEnum.Delete }) == AuthorizationResultEnum.Allowed);
                    }
                }
                else
                {
                    authorized |= currentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin);
                    authorizedDelete |= currentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin);
                }

                if (VirtualContext.ReadonlyMode)
                {
                    authorized = false;
                    authorizedDelete = false;
                }

                // Display form if authorized
                if (authorized || authorizedDelete)
                {
                    // Set visibility of the edit and delete buttons
                    pnlEdit.Visible = true;
                    btnEdit.Visible = btnEdit.Visible && authorized;
                    btnDelete.Visible = btnDelete.Visible && AllowDelete && authorizedDelete;
                    editForm.LogActivity = LogActivity;
                    editForm.ComponentName = WebPartID;

                    if (pnlForm.Visible)
                    {
                        editForm.StopProcessing = false;
                        editForm.AllowDelete = AllowDelete;
                        editForm.CheckPermissions = CheckPermissions;
                        editForm.NodeID = node.NodeID;
                        editForm.SiteName = SiteName;
                        editForm.CultureCode = CultureCode;
                        editForm.AlternativeFormName = AlternativeFormName;
                        editForm.ValidationErrorMessage = ValidationErrorMessage;
                        editForm.CMSForm.IsLiveSite = true;

                        // Reload data
                        editForm.ReloadData(false);
                    }

                    editForm.OnAfterApprove += editForm_OnAfterChange;
                    editForm.OnAfterReject += editForm_OnAfterChange;
                    editForm.OnAfterDelete += editForm_OnAfterChange;
                    editForm.CMSForm.OnAfterSave += CMSForm_OnAfterSave;
                }
                else
                {
                    // Not authorized
                    editForm.StopProcessing = true;
                }
            }
        }
    }


    /// <summary>
    /// Reloads the data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// On btnEdit click event handler.
    /// </summary>
    protected void btnEdit_Click(object sender, EventArgs e)
    {
        // Close edit form
        if (pnlForm.Visible)
        {
            pnlForm.Visible = false;
            btnDelete.Visible = true;

            // Refresh current page
            URLHelper.Redirect(RequestContext.RawURL);
        }
        // Show edit form
        else
        {
            editForm.Action = "edit";
            pnlForm.Visible = true;
            btnDelete.Visible = false;

            btnEdit.CssClass = "EditContributionClose";
        }

        ReloadData();
    }


    /// <summary>
    /// On btnDelete click event handler.
    /// </summary>
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        // Close delete form
        if (pnlForm.Visible)
        {
            pnlForm.Visible = false;
            btnEdit.Visible = true;

            btnDelete.CssClass = "EditContributionDelete";
            btnEdit.CssClass = "EditContributionEdit";
        }
        // Show delete form
        else
        {
            editForm.Action = "delete";
            pnlForm.Visible = true;

            btnEdit.Visible = false;
            btnDelete.CssClass = "EditContributionClose";
        }

        ReloadData();
    }


    /// <summary>
    /// EditForm after change event handler.
    /// </summary>
    private void editForm_OnAfterChange(object sender, EventArgs e)
    {
        CMSForm_OnAfterSave(sender, e);
    }


    /// <summary>
    /// CMSForm after save event handler.
    /// </summary>
    private void CMSForm_OnAfterSave(object sender, EventArgs e)
    {
        if (!StandAlone)
        {
            // Reload data after saving the document
            PagePlaceholder.ClearCache();
            PagePlaceholder.ReloadData();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (!pnlEdit.Visible)
        {
            // Hide control
            Visible = false;
        }
        else
        {
            // Set resource strings and CSS classes of the edit and delete buttons
            if (!pnlForm.Visible)
            {
                btnEdit.ResourceString = HTMLHelper.HTMLEncode(EditButtonText);
                btnEdit.CssClass = "EditContributionEdit";
                btnDelete.ResourceString = HTMLHelper.HTMLEncode(DeleteButtonText);
                btnDelete.CssClass = "EditContributionDelete";
            }
            else
            {
                if (editForm.Action == "edit")
                {
                    btnEdit.ResourceString = HTMLHelper.HTMLEncode(CloseEditModeButtonText);
                }
                else
                {
                    btnDelete.ResourceString = HTMLHelper.HTMLEncode(CloseEditModeButtonText);
                }
            }
        }

        base.OnPreRender(e);
    }


    private TreeNode GetDocument(UserInfo user)
    {
        var tree = new TreeProvider(user);
        var node = tree.SelectSingleNode(SiteName, MacroResolver.ResolveCurrentPath(Path), CultureCode, false, null, false, CheckPermissions);

        if ((node != null) && (PortalContext.ViewMode != ViewModeEnum.LiveSite))
        {
            node = DocumentHelper.GetDocument(node, tree);
        }

        return node;
    }

    #endregion
}