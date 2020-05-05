using System;
using System.Web.UI.WebControls;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.SocialMarketing;


/// <summary>
/// Represents a control that allows the user to select a single item from a list of available URL shorteners.
/// </summary>
public partial class CMSModules_SocialMarketing_FormControls_AvailableUrlShortenerSelector : FormEngineUserControl
{

    #region "Constants and fields"

    private const string SOCIALNETWORKNAME_PROPERTYNAME = "SocialNetworkName";
    private int? mSiteID;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets a name of the social network that the URL shortener is selected for.
    /// </summary>
    public SocialNetworkTypeEnum SocialNetworkName
    {
        get
        {
            var socialNetworkName = ValidationHelper.GetString(GetValue(SOCIALNETWORKNAME_PROPERTYNAME), null);
            SocialNetworkTypeEnum socialNetworkNameEnum;
            if (Enum.TryParse<SocialNetworkTypeEnum>(socialNetworkName, out socialNetworkNameEnum))
            {
                return socialNetworkNameEnum;
            }
            return SocialNetworkTypeEnum.None;
        }
        set
        {
            SetValue(SOCIALNETWORKNAME_PROPERTYNAME, value);
        }
    }


    /// <summary>
    /// Gets or sets Site ID.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteID ?? (mSiteID = SiteContext.CurrentSiteID).Value;
        }
        set
        {
            mSiteID = value;
        }
    }


    /// <summary>
    /// Gets or sets a value indicating whether the control is enabled.
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
            UrlShortenerDropDownList.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets a form control value.
    /// </summary>
    /// <remarks>
    /// The control uses Int32 values that represents URLShortenerTypeEnum items.
    /// </remarks>
    public override object Value
    {
        get
        {
            URLShortenerTypeEnum shortener = URLShortenerTypeEnum.None;
            if (Enum.TryParse<URLShortenerTypeEnum>(UrlShortenerDropDownList.SelectedValue, true, out shortener))
            {
                return (int)shortener;
            }
            return (int)URLShortenerTypeEnum.None;
        }
        set
        {
            EnsureDropDownListItems();
            if (value != null)
            {
                URLShortenerTypeEnum selectedShortener = value is URLShortenerTypeEnum ? (URLShortenerTypeEnum)value : (URLShortenerTypeEnum)ValidationHelper.GetInteger(value, -1);
                if (!Enum.IsDefined(typeof(URLShortenerTypeEnum), selectedShortener))
                {
                    selectedShortener = URLShortenerTypeEnum.None;
                }
                UrlShortenerDropDownList.SelectedValue = Enum.GetName(typeof(URLShortenerTypeEnum), selectedShortener);
            }
        }
    }

    #endregion


    #region "Life-cycle methods"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        EnsureDropDownListItems();
    }

    #endregion


    #region "Private methods"

    private void EnsureDropDownListItems()
    {
        if (UrlShortenerDropDownList.Items.Count == 0)
        {
            InitializeDropDownList();
        }
    }


    private void InitializeDropDownList()
    {
        foreach (URLShortenerTypeEnum shortener in URLShortenerHelper.GetAvailableURLShorteners(SiteID))
        {
            UrlShortenerDropDownList.Items.Add(CreateDropDownListItem(shortener));
        }

        if (SocialNetworkName != SocialNetworkTypeEnum.None)
        {
            URLShortenerTypeEnum defaultShortener = URLShortenerHelper.GetDefaultURLShortenerForSocialNetwork(SocialNetworkName, SiteID);
            UrlShortenerDropDownList.SelectedValue = Enum.GetName(typeof(URLShortenerTypeEnum), defaultShortener);
        }
    }


    private ListItem CreateDropDownListItem(URLShortenerTypeEnum shortener)
    {
        string shortenerName = Enum.GetName(typeof(URLShortenerTypeEnum), shortener);
        return new ListItem
        {
            Value = shortenerName,
            Text = ResHelper.GetString(String.Format("urlshortenertypeenum.{0}", shortenerName))
        };
    }

    #endregion

}