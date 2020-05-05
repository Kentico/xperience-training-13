using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


[Help("custom_tables_edit_item", "helpTopic")]
[UIElement(ModuleName.CUSTOMTABLES, "CustomTables")]
public partial class CMSModules_CustomTables_Tools_CustomTable_Data_EditItem : CMSCustomTablesToolsPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
        RequireSite = false;

        bool accessGranted = true;

        // Get custom table id from url
        int customTableId = QueryHelper.GetInteger("objectid", 0);

        if (customTableId > 0)
        {
            // Get custom table item id
            int itemId = QueryHelper.GetInteger("itemid", 0);

            // Running in site manager?
            bool siteManager = QueryHelper.GetInteger("sm", 0) == 1;

            DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(customTableId);
            // Set edited object
            EditedObject = dci;

            // If class exists
            if ((dci != null) && dci.ClassIsCustomTable)
            {
                // Ensure that object belongs to current site or user has access to site manager
                if (!CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin) && (dci.AssignedSites[SiteContext.CurrentSiteName] == null))
                {
                    RedirectToInformation(GetString("general.notassigned"));
                }

                // Edit item
                if (itemId > 0)
                {
                    // Check 'Read' permission
                    if (!dci.CheckPermissions(PermissionsEnum.Read, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser))
                    {
                        ShowError(String.Format(GetString("customtable.permissiondenied.read"), dci.ClassName));
                        plcContent.Visible = false;
                        accessGranted = false;
                    }

                    if (!siteManager)
                    {
                        PageTitle.TitleText = GetString("customtable.data.edititemtitle");
                    }
                }
                // New item
                else
                {
                    if (!siteManager)
                    {
                        PageTitle.TitleText = GetString("customtable.data.newitemtitle");
                    }
                }

                string listPage = "~/CMSModules/Customtables/Tools/CustomTable_Data_List.aspx";
                string newItemPage = "~/CMSModules/CustomTables/Tools/CustomTable_Data_EditItem.aspx";

                if (QueryHelper.GetString("saved", String.Empty) != String.Empty)
                {
                    // If this was creating of new item show the link again
                    if ((QueryHelper.GetString("new", String.Empty) != String.Empty))
                    {
                        // Create another link
                        CurrentMaster.HeaderActions.AddAction(new HeaderAction
                        {
                            Text = GetString("customtable.data.createanother"),
                            RedirectUrl = ResolveUrl(newItemPage + "?new=1&objectid=" + customTableId + (siteManager ? "&sm=1" : String.Empty))
                        });
                    }
                }

                // Create breadcrumbs
                CreateBreadcrumbs(siteManager, dci, listPage, customTableId, itemId);

                // Set edit form
                if (accessGranted)
                {
                    customTableForm.CustomTableId = customTableId;
                    customTableForm.ItemId = itemId;
                    customTableForm.EditItemPageAdditionalParams = (siteManager ? "sm=1" : String.Empty);
                }
            }
            else
            {
                ShowError(GetString("customtable.notcustomtable"));
            }
        }
    }


    /// <summary>
    /// Creates breadcrumbs
    /// </summary>
    /// <param name="siteManager">Indicates if used in SM</param>
    /// <param name="classInfo">Class info</param>
    /// <param name="listPage">List page URL</param>
    /// <param name="customTableId">Custom table ID</param>
    /// <param name="itemId">Item ID</param>
    private void CreateBreadcrumbs(bool siteManager, DataClassInfo classInfo, string listPage, int customTableId, int itemId)
    {
        // Initializes page title
        if (!siteManager)
        {
            PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
            {
                Text = GetString("customtable.list.title"),
                RedirectUrl = "~/CMSModules/Customtables/Tools/CustomTable_List.aspx"
            });
        }
        PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
        {
            Text = classInfo.ClassDisplayName,
            RedirectUrl = listPage + "?objectid=" + customTableId + (siteManager ? "&sm=1" : String.Empty)
        });
        PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
        {
            Text = itemId > 0 ? GetString("customtable.data.Edititem") : GetString("customtable.data.NewItem")
        });

        // Do not include type as breadcrumbs suffix
        UIHelper.SetBreadcrumbsSuffix("");
    }
}
