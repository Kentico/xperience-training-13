using System;
using System.Web;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;


public partial class CMSModules_PortalEngine_FormControls_SelectContainer : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            selectContainer.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return selectContainer.Value;
        }
        set
        {
            EnsureChildControls();
            selectContainer.Value = value;
        }
    }


    /// <summary>
    /// Gets ClientID of the CMSDropDownList with containers.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return selectContainer.ClientID;
        }
    }


    /// <summary>
    /// Is live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return selectContainer.IsLiveSite;
        }
        set
        {
            selectContainer.IsLiveSite = value;
        }
    }

    #endregion


    #region "Methods"

    protected override void CreateChildControls()
    {
        base.CreateChildControls();

        selectContainer.AdditionalDropDownCSSClass = "SelectorDropDown";

        selectContainer.WhereCondition = string.Format("ContainerID IN (SELECT ContainerID FROM CMS_WebPartContainerSite WHERE SiteID = {0})", SiteContext.CurrentSiteID);

        // Add none value
        selectContainer.SpecialFields.Add(new SpecialField() { Text = GetString("general.empty"), Value = String.Empty });

        var currentUser = MembershipContext.AuthenticatedUser;

        // Check user permissions
        bool deskAuthorized = currentUser.IsAuthorizedPerUIElement("CMS.Content", "Content");

        if (!IsLiveSite && deskAuthorized)
        {
            // Initialize selector
            SetDialog();
        }
    }


    private void SetDialog()
    {
        string aliasPath = QueryHelper.GetString("aliaspath", String.Empty);
        Guid instanceGUID = QueryHelper.GetGuid("instanceguid", Guid.Empty);

        if (aliasPath != String.Empty)
        {
            selectContainer.AdditionalUrlParameters = "&aliaspath=" + HttpUtility.UrlEncode(aliasPath);
        }

        if (instanceGUID != Guid.Empty)
        {
            selectContainer.AdditionalUrlParameters += "&instanceGUID=" + instanceGUID;
        }

        selectContainer.ElementResourceName = "CMS.Design";
        selectContainer.EditItemElementName = "EditWebPartContainer";
        selectContainer.NewItemElementName = "NewWebPartContainer";

        selectContainer.EditDialogWindowWidth = 1070;
        selectContainer.EditDialogWindowHeight = 770;
    }

    #endregion
}