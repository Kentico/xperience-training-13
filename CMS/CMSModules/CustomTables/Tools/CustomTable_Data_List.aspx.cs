using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.CustomTables;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


[UIElement("CMS.CustomTables", "CustomTables")]
public partial class CMSModules_CustomTables_Tools_CustomTable_Data_List : CMSCustomTablesToolsPage
{
    protected int customTableId = 0;
    protected string formName = String.Empty;


    protected void Page_Init(object sender, EventArgs e)
    {
        RequireSite = false;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        string newItemPage = "~/CMSModules/CustomTables/Tools/CustomTable_Data_EditItem.aspx";

        // Get form ID from url
        customTableId = QueryHelper.GetInteger("objectid", 0);

        // Running in site manager?
        bool siteManager = QueryHelper.GetInteger("sm", 0) == 1;

        DataClassInfo dci = null;

        // Read data only if user is site manager global admin or table is bound to current site
        if (CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin) || (ClassSiteInfo.Provider.Get(customTableId, SiteContext.CurrentSiteID) != null))
        {
            // Get CustomTable class
            dci = DataClassInfoProvider.GetDataClassInfo(customTableId);
        }

        // Set edited object
        EditedObject = dci;

        if ((dci != null) && dci.ClassIsCustomTable)
        {
            customTableDataList.CustomTableClassInfo = dci;
            customTableDataList.EditItemPageAdditionalParams = (siteManager ? "sm=1" : String.Empty);
            customTableDataList.ViewItemPageAdditionalParams = (siteManager ? "sm=1" : String.Empty);
            // Set alternative form and data container
            customTableDataList.UniGrid.FilterFormName = dci.ClassName + ".filter";
            customTableDataList.UniGrid.FilterFormData = CustomTableItem.New(dci.ClassName);

            ScriptHelper.RegisterDialogScript(this);
            ScriptHelper.RegisterClientScriptBlock(this, typeof (string), "SelectFields", ScriptHelper.GetScript("function SelectFields() { modalDialog('" +
                                                                                                                       ResolveUrl("~/CMSModules/CustomTables/Tools/CustomTable_Data_SelectFields.aspx") + "?customtableid=" + customTableId + "'  ,'CustomTableFields', 500, 500); }"));
            
            if (!siteManager)
            {
                PageTitle.TitleText = GetString("customtable.edit.header");
            }

            // Check 'Read' permission
            if (!dci.CheckPermissions(PermissionsEnum.Read, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser))
            {
                ShowError(String.Format(GetString("customtable.permissiondenied.read"), dci.ClassName));
                plcContent.Visible = false;
                return;
            }

            // New item link
            bool canCreate = dci.CheckPermissions(PermissionsEnum.Create, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser);
            HeaderActions.AddAction(new HeaderAction
            {
                Text = GetString("customtable.data.newitem"),
                RedirectUrl = ResolveUrl(newItemPage + "?new=1&objectid=" + customTableId + (siteManager ? "&sm=1" : "")),
                Enabled = canCreate,
                Tooltip = canCreate ? String.Empty : String.Format(GetString("customtable.permissiondenied.create"), dci.ClassName)
            });

            // Select fields link
            HeaderActions.AddAction(new HeaderAction
            {
                Text = GetString("customtable.data.selectdisplayedfields"),
                OnClientClick = "SelectFields();",
                ButtonStyle = ButtonStyle.Default,
            });

            if (!siteManager)
            {
                // Initializes page title
                PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
                {
                    Text = GetString("customtable.list.title"),
                    RedirectUrl = "~/CMSModules/Customtables/Tools/CustomTable_List.aspx"
                });
                PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
                {
                    Text = dci.ClassDisplayName
                });
            }
        }
        else
        {
            customTableDataList.StopProcessing = true;
            customTableDataList.Visible = false;

            ShowError(GetString("customtable.notcustomtable"));
        }
    }
}
