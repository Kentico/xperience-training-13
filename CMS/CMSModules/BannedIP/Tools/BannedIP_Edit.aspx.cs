using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Protection;
using CMS.SiteProvider;
using CMS.UIControls;

// Title
[Title("banip.editHeaderCaption")]
[UIElement(BannedIPInfo.OBJECT_TYPE, "BannedIP")]
public partial class CMSModules_BannedIP_Tools_BannedIP_Edit : CMSBannedIPsPage
{
    protected int itemid = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Controls initialization
        radBanIP.Text = GetString("banip.radBanIP");
        radAllowIP.Text = GetString("banip.radAllowIP");

        lblIPAddressBanType.Text = GetString("banip.IPAddressBanType") + ResHelper.Colon;
        lblIPAddressBanEnabled.Text = GetString("general.enabled") + ResHelper.Colon;
        lblIPAddress.Text = GetString("banip.IPAddress") + ResHelper.Colon;
        lblIPAddressBanReason.Text = GetString("banip.IPAddressBanReason") + ResHelper.Colon;

        rfvIPAddress.ErrorMessage = GetString("banip.IPAddressEmpty");
        lblIPAddressAllowOverride.Text = GetString("banip.IPAddressAllowOverride") + ResHelper.Colon;

        if (!RequestHelper.IsPostBack())
        {
            // Add list items to ban type drop down list
            ControlsHelper.FillListControlWithEnum<BanControlEnum>(drpIPAddressBanType, "banip.bantype", useStringRepresentation: true);
            drpIPAddressBanType.SelectedValue = BanControlEnum.AllNonComplete.ToStringRepresentation();
        }

        string currentBannedIP = GetString("banip.NewItemCaption");

        // Get bannedIP id from querystring		
        itemid = QueryHelper.GetInteger("itemid", 0);
        if (itemid > 0)
        {
            BannedIPInfo bannedIPObj = BannedIPInfoProvider.GetBannedIPInfo(itemid);
            EditedObject = bannedIPObj;

            if (bannedIPObj != null)
            {
                //Check whether the item truly belogs to specified site
                if (((SiteID > 0) && (bannedIPObj.IPAddressSiteID != SiteID)) ||
                    ((SelectedSiteID > 0) && (bannedIPObj.IPAddressSiteID != SelectedSiteID)))
                {
                    RedirectToAccessDenied(GetString("banip.invaliditem"));
                }

                currentBannedIP = bannedIPObj.IPAddress;

                // Add site info to breadcrumbs
                if (SiteID == 0)
                {
                    if (bannedIPObj.IPAddressSiteID == 0)
                    {
                        currentBannedIP += " (global)";
                        radAllowIP.Text = GetString("banip.radAllowIPglobal");

                        plcIPOveride.Visible = true;
                    }
                    else
                    {
                        SiteInfo si = SiteInfoProvider.GetSiteInfo(bannedIPObj.IPAddressSiteID);
                        if (si != null)
                        {
                            currentBannedIP += " (" + si.DisplayName + ")";
                        }
                    }
                }

                // Fill editing form
                if (!RequestHelper.IsPostBack())
                {
                    LoadData(bannedIPObj);

                    // Show that the bannedIP was created or updated successfully
                    if ((QueryHelper.GetInteger("saved", 0) == 1) && !RequestHelper.IsPostBack())
                    {
                        ShowChangesSaved();
                    }
                }
            }
        }

        // Initializes page title control		
        SetBreadcrumb(0, GetString("banip.listHeaderCaption"), ResolveUrl("BannedIP_List.aspx?siteId=" + SiteID + "&selectedsiteid=" + SelectedSiteID), null, null);
        SetBreadcrumb(1, currentBannedIP, null, null, null);

        // Add info about selected site in Site manager for new item
        if ((SiteID == 0) && (itemid == 0))
        {
            if (SelectedSiteID > 0)
            {
                // Site banned IP
                SiteInfo si = SiteInfoProvider.GetSiteInfo(SelectedSiteID);
                if (si != null)
                {
                    SetBreadcrumb(1, currentBannedIP + " (" + si.DisplayName + ")", null, null, null);
                }
            }
            else
            {
                // Global banned IP
                SetBreadcrumb(1, currentBannedIP + " (global)", null, null, null);

                radAllowIP.Text = GetString("banip.radAllowIPglobal");

                plcIPOveride.Visible = true;
            }
        }

        // Different header and icon if it is new item
        if (itemid <= 0)
        {
            SetTitle(GetString("banip.newHeaderCaption"));
        }
    }


    /// <summary>
    /// Load data of editing bannedIP.
    /// </summary>
    /// <param name="bannedIPObj">BannedIP object</param>
    protected void LoadData(BannedIPInfo bannedIPObj)
    {
        // Check proper radio button
        if (bannedIPObj.IPAddressAllowed)
        {
            radAllowIP.Checked = true;
        }
        else
        {
            radBanIP.Checked = true;
        }

        drpIPAddressBanType.SelectedValue = bannedIPObj.IPAddressBanType;
        chkIPAddressBanEnabled.Checked = bannedIPObj.IPAddressBanEnabled;

        txtIPAddress.Text = bannedIPObj.IPAddress;
        txtIPAddressBanReason.Text = bannedIPObj.IPAddressBanReason;

        if (SiteID == 0)
        {
            chkIPAddressAllowOverride.Checked = bannedIPObj.IPAddressAllowOverride;
        }
    }


    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Check 'Modify' permission
        CheckPermissions("Modify");

        string errorMessage = new Validator().NotEmpty(txtIPAddress.Text, GetString("banip.IPAddressEmpty")).Result;

        // Check if regularized ip address doesn't overflow database column
        if (BannedIPInfoProvider.GetRegularIPAddress(txtIPAddress.Text).Length > 200)
        {
            errorMessage = GetString("banip.IPAddressInvalid");
        }

        if (errorMessage == "")
        {
            // If bannedIP doesn't already exist, create new one
            BannedIPInfo bannedIPObj = BannedIPInfoProvider.GetBannedIPInfo(itemid) ?? new BannedIPInfo();

            bannedIPObj.IPAddressAllowed = radAllowIP.Checked;
            bannedIPObj.IPAddressBanType = drpIPAddressBanType.SelectedValue;
            bannedIPObj.IPAddressBanEnabled = chkIPAddressBanEnabled.Checked;
            bannedIPObj.IPAddress = txtIPAddress.Text.Trim();

            // Make sure text is not too long
            bannedIPObj.IPAddressBanReason = TextHelper.LimitLength(txtIPAddressBanReason.Text.Trim(), txtIPAddressBanReason.MaxLength);

            if (SiteID == 0)
            {
                // For (global) set overriding from checkbox, otherwise is true
                bannedIPObj.IPAddressAllowOverride = (SelectedSiteID > 0) || chkIPAddressAllowOverride.Checked;

                // If site selected assign it to banned IP
                if (SelectedSiteID > 0)
                {
                    bannedIPObj.IPAddressSiteID = SelectedSiteID;
                }
            }
            else
            {
                // Default setting for editing from CMSDesk
                bannedIPObj.IPAddressAllowOverride = true;
                bannedIPObj.IPAddressSiteID = SiteID;
            }

            BannedIPInfoProvider.SetBannedIPInfo(bannedIPObj);

            URLHelper.Redirect(UrlResolver.ResolveUrl("Bannedip_Edit.aspx?siteid=" + SiteID + "&selectedsiteid=" + SelectedSiteID + "&itemid=" + bannedIPObj.IPAddressID + "&saved=1"));
        }
        else
        {
            ShowError(errorMessage);
        }
    }
}