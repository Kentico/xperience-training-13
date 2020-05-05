using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_Community_Groups_GroupsFilter : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the filter button text.
    /// </summary>
    public string ButtonText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ButtonText"), "");
        }
        set
        {
            SetValue("ButtonText", value);
            filterGroups.ButtonText = value;
        }
    }


    /// <summary>
    /// Gets or sets the group name link text.
    /// </summary>
    public string SortGroupNameLinkText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SortGroupNameLinkText"), "");
        }
        set
        {
            SetValue("SortGroupNameLinkText", value);
            filterGroups.SortGroupNameLinkText = value;
        }
    }


    /// <summary>
    /// Gets or sets the group created link text.
    /// </summary>
    public string SortGroupCreatedLinkText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SortGroupCreatedLinkText"), "");
        }
        set
        {
            SetValue("SortGroupCreatedLinkText", value);
            filterGroups.SortGroupCreatedLinkText = value;
        }
    }


    /// <summary>
    /// Gets or sets the filter button text.
    /// </summary>
    public bool DisableFilterCaching
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisableFilterCaching"), false);
        }
        set
        {
            SetValue("DisableFilterCaching", value);
            filterGroups.DisableFilterCaching = value;
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
    /// Initializes the control properties.
    /// </summary>
    public void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            filterGroups.FilterName = ValidationHelper.GetString(GetValue("WebPartControlID"), ID);
            filterGroups.ButtonText = ButtonText;
            filterGroups.SortGroupCreatedLinkText = SortGroupCreatedLinkText;
            filterGroups.SortGroupNameLinkText = SortGroupNameLinkText;
            filterGroups.DisableFilterCaching = DisableFilterCaching;
        }
    }
}