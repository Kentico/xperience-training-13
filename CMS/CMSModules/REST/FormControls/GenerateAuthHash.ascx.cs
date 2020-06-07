using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;

public partial class CMSModules_REST_FormControls_GenerateAuthHash : FormEngineUserControl
{
    private SiteInfo mSelectedSite;


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return null;
        }
        set
        {
        }
    }


    /// <summary>
    /// Gets selected site.
    /// </summary>
    /// <remarks>
    /// Gets selected site from query string, if not found it fallbacks to
    /// <see cref="SiteContext.CurrentSite" />.
    /// </remarks>
    private SiteInfo SelectedSite
    {
        get
        {
            if (mSelectedSite == null)
            {
                var siteId = QueryHelper.GetInteger("siteId", -1);
                mSelectedSite = (siteId > 0)
                    ? SiteInfo.Provider.Get(siteId)
                    : SiteContext.CurrentSite;
            }

            return mSelectedSite;
        }
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        return true;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterDialogScript(Page);
        
        if (SelectedSite == null)
        {
            lnkGenerate.Enabled = lnkInvalidate.Enabled = false;
            return;
        }

        lnkGenerate.OnClientClick = "modalDialog('" + UrlResolver.ResolveUrl("~/CMSModules/REST/FormControls/GenerateHash.aspx") + "' , 'GenerateAuthHash', 800, 410); return false;";
        lnkInvalidate.OnClientClick = $"return confirm('{ResHelper.GetStringFormat("rest.invalidatehash.confirmation", HTMLHelper.EncodeForHtmlAttribute(SelectedSite.DisplayName))}');";

        lnkInvalidate.Click += (s, args) =>
        {
            SettingsKeyInfoProvider.SetValue("CMSRESTUrlHashSalt", SelectedSite.SiteName, Guid.NewGuid());
            lblInvalidate.Visible = true;
        };

        lblInvalidate.Text = ResHelper.GetStringFormat("rest.invalidatehash.message", HTMLHelper.HTMLEncode(SelectedSite.DisplayName));
    }
}
