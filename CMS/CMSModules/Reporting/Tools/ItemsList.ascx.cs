using System;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.Reporting;
using CMS.UIControls;


public partial class CMSModules_Reporting_Tools_ItemsList : CMSUserControl
{
    #region "Public properties"

    /// <summary>
    /// URL of the edit page.
    /// </summary>
    public string EditUrl
    {
	    get;
	    set;
    }


    /// <summary>
    /// Gets or sets report id, which is used for loading the report if Report property is null.
    /// </summary>
    public int ReportID
    {
	    get;
	    set;
    }


    /// <summary>
    /// Gets or sets the report object to load the items.
    /// </summary>
    public ReportInfo Report
    {
	    get;
	    set;
    }


    /// <summary>
    /// Item type.
    /// </summary>
    public ReportItemType ItemType
    {
	    get;
	    set;
    }

    #endregion


    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check 'Read' permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.reporting", "Read"))
        {
            CMSPage.RedirectToAccessDenied("cms.reporting", "Read");
        }

        if (Report != null)
        {
            brsItems.ReportID = Report.ReportID;
        }

        brsItems.ReportType = ItemType;
        brsItems.Display = false;
        brsItems.IsLiveSite = IsLiveSite;
        brsItems.ShowItemSelector = true;

        // Enable page methods to get item name
        ScriptManager scriptManager = ScriptManager.GetCurrent(Page);
        scriptManager.EnablePageMethods = true;
    }


    /// <summary>
    /// Pre render.
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        Initialize();
    }


    /// <summary>
    /// Sets buttons actions.
    /// </summary>
    protected void Initialize()
    {
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "GetSelectedItem" + ItemType, ScriptHelper.GetScript(@"
            function getSelectedItem_" + ItemType + @"() { 
				if (document.getElementById('" + brsItems.UniSelectorClientID + @"') != null) {
					return document.getElementById('" + brsItems.UniSelectorClientID + @"').value;
				}
				return 0;
			}

            function DeleteItem_" + ItemType + @"() {
				if ((document.getElementById('" + brsItems.UniSelectorClientID + @"') != null) && (document.getElementById('" + brsItems.UniSelectorClientID + @"').value != '0')) {
					if (confirm(" + ScriptHelper.GetLocalizedString("general.confirmdelete") + @")) {
						document.getElementById('" + hdnItemId.ClientID + @"').value = getSelectedItem_" + ItemType + @"(); 
						" + Page.ClientScript.GetPostBackEventReference(btnHdnDelete, null) + @" 
					}
				} else { 
					alert(" + ScriptHelper.GetLocalizedString("Reporting_General.SelectObjectFirst") + @"); 
				}
			}

            function InserMacro_" + ItemType + @"() {
				if ((document.getElementById('" + brsItems.UniSelectorClientID + @"') != null) && (document.getElementById('" + brsItems.UniSelectorClientID + @"').value != '0')) {
					PageMethods.GetReportItemName('" + ReportInfoProvider.ReportItemTypeToString(ItemType) + "',getSelectedItem_" + ItemType + "(),OnComplete_" + ItemType + @");
				} else { 
					alert(" + ScriptHelper.GetLocalizedString("Reporting_General.SelectObjectFirst") + @"); 
				}
            }

			function OnComplete_" + ItemType + @"(result, response, context) {
				InsertHTML('%%control:Report" + ItemType + "?" + Report.ReportName + @".' + result +'%%');
            }

            function CloneItem_" + ItemType + @"(id) {
				if ((document.getElementById('" + brsItems.UniSelectorClientID + @"') != null) && (document.getElementById('" + brsItems.UniSelectorClientID + @"').value != '0')) { 
					modalDialog('" + UrlResolver.ResolveUrl("~/CMSModules/Objects/Dialogs/CloneObjectDialog.aspx?objectType=" + GetObjectType() + @"&objectId=' + id") + @", 'CloneObject', 750, 470);
				} else { 
					alert(" + ScriptHelper.GetLocalizedString("Reporting_General.SelectObjectFirst") + @"); 
				}
            }"));


        const string MODAL_HEIGHT = "'85%'";
        const string MODAL_WIDTH = "950";


		if (Report != null)
		{
			string baseUrl = URLHelper.AddParameterToUrl(ResolveUrl(EditUrl), "reportId", Report.ReportID.ToString());
			string fullUrl = ApplicationUrlHelper.AppendDialogHash(baseUrl);

			btnAdd.Actions.Add(new CMSButtonAction()
			{
				OnClientClick = "modalDialog('" + fullUrl + "','ReportItemEdit'," + MODAL_WIDTH + "," + MODAL_HEIGHT + ");return false;",
				Text = GetString("general.new")
			});

			fullUrl = URLHelper.AddParameterToUrl(baseUrl, "preview", "true");
			fullUrl = ApplicationUrlHelper.AppendDialogHash(fullUrl);

			btnAdd.Actions.Add(new CMSButtonAction()
			{
				OnClientClick = "if (getSelectedItem_" + ItemType + "() != '0') { modalDialog('" + fullUrl + "&objectid='+ getSelectedItem_" + ItemType + "(),'ReportItemEdit'," + MODAL_WIDTH + "," + MODAL_HEIGHT + "); } else { alert(" + ScriptHelper.GetLocalizedString("Reporting_General.SelectObjectFirst") + ");} return false;",
				Text = GetString("general.preview")
			});

			fullUrl = ApplicationUrlHelper.AppendDialogHash(baseUrl);

			btnAdd.Actions.Add(new CMSButtonAction()
			{
				OnClientClick = "if (getSelectedItem_" + ItemType + "() != '0') { modalDialog('" + fullUrl + "&objectid='+ getSelectedItem_" + ItemType + "(),'ReportItemEdit'," + MODAL_WIDTH + "," + MODAL_HEIGHT + "); } else { alert(" + ScriptHelper.GetLocalizedString("Reporting_General.SelectObjectFirst") + ");} return false;",
				Text = GetString("general.edit")
			});

			btnAdd.Actions.Add(new CMSButtonAction()
			{
				OnClientClick = "DeleteItem_" + ItemType + "(); return false;",
				Text = GetString("general.delete")
			});

			btnAdd.Actions.Add(new CMSButtonAction()
			{
				OnClientClick = "CloneItem_" + ItemType + @"(getSelectedItem_" + ItemType + "()); return false;",
				Text = GetString("general.clone")
			});

			btnInsert.OnClientClick = "InserMacro_" + ItemType + "(); return false;";
		}
	}


    /// <summary>
    /// Returns current object type
    /// </summary>
    protected string GetObjectType()
    {
        // Switch by item type
        switch (ItemType)
        {
            // Graph
            case ReportItemType.Graph:
            case ReportItemType.HtmlGraph:
                return ReportGraphInfo.TYPEINFO.ObjectType;

            // Table
            case ReportItemType.Table:
                return ReportTableInfo.TYPEINFO.ObjectType;

            // Value
            case ReportItemType.Value:
                return ReportValueInfo.TYPEINFO.ObjectType;
        }

        return String.Empty;
    }


    protected void btnHdnDelete_Click(object sender, EventArgs e)
    {
        // Check 'Modify' permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.reporting", "Modify"))
        {
            CMSPage.RedirectToAccessDenied("cms.reporting", "Modify");
        }

        // Check whether object is defined
        if (!String.IsNullOrEmpty(hdnItemId.Value))
        {
            // Get id
            int id = ValidationHelper.GetInteger(hdnItemId.Value, 0);

            // Switch by type
            switch (ItemType)
            {
                // Graph
                case ReportItemType.Graph:
                case ReportItemType.HtmlGraph:
                    ReportGraphInfoProvider.DeleteReportGraphInfo(id);
                    break;

                // Table
                case ReportItemType.Table:
                    ReportTableInfoProvider.DeleteReportTableInfo(id);
                    break;

                // Value
                case ReportItemType.Value:
                    ReportValueInfoProvider.DeleteReportValueInfo(id);
                    break;
            }
        }
    }
}
