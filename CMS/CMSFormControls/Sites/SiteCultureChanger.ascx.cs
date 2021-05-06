using System;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Localization;
using CMS.Search;
using CMS.SiteProvider;


public partial class CMSFormControls_Sites_SiteCultureChanger : FormEngineUserControl
{
    /// <summary>
    /// Value of the control.
    /// </summary>
    public override object Value
    {
        get
        {
            return txtCulture.Text;
        }
        set
        {
            txtCulture.Text = ValidationHelper.GetString(value, String.Empty);
        }
    }


    /// <summary>
    /// Page load.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        SiteInfo si = EditedObject as SiteInfo;
        if (si == null)
        {
            return;
        }

        CultureInfo ci = CultureInfo.Provider.Get(CultureHelper.GetDefaultCultureCode(si.SiteName));
        string currentCulture = ci.CultureCode;
        if (!RequestHelper.IsPostBack())
        {
            txtCulture.Text = ci.CultureName;
        }

        // Set the culture textbox readonly
        txtCulture.Attributes.Add("readonly", "readonly");

        btnChange.Text = GetString("general.change");
        btnChange.OnClientClick = "OpenCultureChanger('" + si.SiteID + "','" + currentCulture + "'); return false;";
        
        ltlScript.Text = ScriptHelper.GetScript(@"
var pageChangeUrl='" + ResolveUrl("~/CMSModules/Sites/Pages/CultureChange.aspx") + @"'; 
function ChangeCulture(documentChanged) { 
    var hiddenElem = document.getElementById('" + hdnDocumentsChangeChecked.ClientID + @"');
    hiddenElem.value = documentChanged;
    " + Page.ClientScript.GetPostBackEventReference(btnHidden, String.Empty) + @"
} 
");
    }


    /// <summary>
    /// On default culture change.
    /// </summary>
    protected void btnHidden_Click(object sender, EventArgs e)
    {
        SiteInfo si = EditedObject as SiteInfo;

        if (si != null)
        {
            string defaultCultureCode = CultureHelper.GetDefaultCultureCode(si.SiteName);
            if (!String.IsNullOrEmpty(defaultCultureCode))
            {
                CultureInfo ci = CultureInfo.Provider.Get(defaultCultureCode);
                if (ci != null)
                {
                    // Show a message that the action requires rebuilding search index
                    if ((txtCulture.Text != ci.CultureName) && ValidationHelper.GetBoolean(hdnDocumentsChangeChecked.Value, false) && SearchIndexInfoProvider.SearchEnabled)
                    {
                        ShowInformation(String.Format(GetString("general.changessaved") + " " + GetString("srch.indexrequiresrebuild"), "<a href=\"javascript:" + Page.ClientScript.GetPostBackEventReference(this, "saved") + "\">" + GetString("General.clickhere") + "</a>"));
                    }

                    txtCulture.Text = ci.CultureName;
                    btnChange.OnClientClick = "OpenCultureChanger('" + si.SiteID + "','" + ci.CultureCode + "'); return false;";
                }
            }
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        ScriptHelper.RegisterDialogScript(Page);
        base.OnPreRender(e);
    }
}