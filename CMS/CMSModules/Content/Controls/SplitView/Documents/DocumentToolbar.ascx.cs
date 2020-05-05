using System;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Localization;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;


public partial class CMSModules_Content_Controls_SplitView_Documents_DocumentToolbar : CMSUserControl
{
    #region "Properties"

    /// <summary>
    /// Node to work with.
    /// </summary>
    public TreeNode Node
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            string preferredCultureCode = LocalizationContext.PreferredCultureCode;
            string currentSiteName = SiteContext.CurrentSiteName;
            var documentCultures = Node.GetTranslatedCultures();

            // Get site cultures
            DataSet siteCultures = CultureSiteInfoProvider.GetSiteCultures(currentSiteName);
            if (!DataHelper.DataSourceIsEmpty(siteCultures) && (documentCultures.Count > 0))
            {
                string suffixNotTranslated = GetString("SplitMode.NotTranslated");

                foreach (DataRow row in siteCultures.Tables[0].Rows)
                {
                    string cultureCode = ValidationHelper.GetString(row["CultureCode"], null);
                    string cultureName = ResHelper.LocalizeString(ValidationHelper.GetString(row["CultureName"], null));

                    string suffix = string.Empty;

                    // Compare with preferred culture
                    if (CMSString.Compare(preferredCultureCode, cultureCode, true) == 0)
                    {
                        suffix = GetString("SplitMode.Current");
                    }
                    else if (!documentCultures.Contains(cultureCode))
                    {
                        suffix = suffixNotTranslated;
                    }

                    // Add new item
                    var item = new ListItem(cultureName + " " + suffix, cultureCode);
                    drpCultures.Items.Add(item);
                }
            }

            drpCultures.SelectedValue = PortalUIHelper.SplitModeCultureCode;
            drpCultures.Attributes.Add("onchange", "if (parent.CheckChanges('frame2')) { parent.FSP_ChangeCulture(this); }");
        }

        buttons.Actions.Add(new CMSButtonGroupAction { Name = "close", UseIconButton = true, OnClientClick = "FSP_Close();return false;", IconCssClass = "icon-l-list-titles", ToolTip = GetString("splitmode.closelayout") });
        buttons.Actions.Add(new CMSButtonGroupAction { Name = "vertical", UseIconButton = true, OnClientClick = "FSP_Layout('true','frame1','Vertical');return false;", IconCssClass = "icon-l-cols-2 js-split-vertical", ToolTip = GetString("splitmode.verticallayout") });
        buttons.Actions.Add(new CMSButtonGroupAction { Name = "horizontal", UseIconButton = true, OnClientClick = "FSP_Layout(false,'frame1Vertical','Horizontal');;return false;", IconCssClass = "icon-l-rows-2 js-split-horizontal", ToolTip = GetString("splitmode.horizontallayout") });

        // Set css class
        switch (PortalUIHelper.SplitMode)
        {
            case SplitModeEnum.Horizontal:
                buttons.SelectedActionName = "horizontal";
                break;

            case SplitModeEnum.Vertical:
                buttons.SelectedActionName = "vertical";
                break;

            default:
                buttons.SelectedActionName = "close";
                break;
        }

        // Synchronize image
        chckScroll.Checked = PortalUIHelper.SplitModeSyncScrollbars;

        StringBuilder script = new StringBuilder();
        script.Append(
@"
function FSP_Layout(vertical, frameName, cssClassName)
{
    if ((frameName != null) && parent.CheckChanges(frameName)) {
        if (cssClassName != null) {
            var element = document.getElementById('", pnlMain.ClientID, @"');
            if (element != null) {
                element.setAttribute(""class"", 'SplitToolbar ' + cssClassName);
                element.setAttribute(""className"", 'SplitToolbar ' + cssClassName);
            }
        }
        var divRight = document.getElementById('", divRight.ClientID, @"');
        if (vertical) {
            divRight.setAttribute(""class"", 'RightAlign');
            parent.FSP_VerticalLayout();
        }
        else {
            divRight.setAttribute(""class"", '');
            parent.FSP_HorizontalLayout();
        }
    }
}

function FSP_Close() { 
    if (parent.CheckChanges()) { 
        parent.FSP_CloseSplitMode(); 
    }
}
");

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "toolbarScript_" + ClientID, ScriptHelper.GetScript(script.ToString()));

        chckScroll.Attributes.Add("onchange", "javascript:parent.FSP_SynchronizeToolbar()");

        // Set layout
        if (PortalUIHelper.SplitMode == SplitModeEnum.Horizontal)
        {
            pnlMain.CssClass = "SplitToolbar Horizontal";
            divRight.Attributes["class"] = null;
        }
        else if (PortalUIHelper.SplitMode == SplitModeEnum.Vertical)
        {
            pnlMain.CssClass = "SplitToolbar Vertical";
        }

        // Register Init script - FSP_ToolbarInit(selectorId, checkboxId)
        StringBuilder initScript = new StringBuilder();
        initScript.Append("parent.FSP_ToolbarInit('", drpCultures.ClientID, "','", chckScroll.ClientID, "');");

        // Register js scripts
        ScriptHelper.RegisterJQuery(Page);
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "FSP_initToolbar", ScriptHelper.GetScript(initScript.ToString()));
    }

    #endregion
}
