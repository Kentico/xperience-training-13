using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;


public partial class CMSModules_Modules_Controls_Settings_Category_SettingsGroup : SettingsGroup
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Unigrid
        gridElem.HideControlForZeroRows = false;
        gridElem.OrderBy = "KeyOrder";
        gridElem.OnAction += KeyAction;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.ZeroRowsText = GetString("settings.group.nokeysfound");

        if (Category != null)
        {
            string catIdStr = Category.CategoryID.ToString();

            // Action buttons
            var rightPanel = cpCategory.RightPanel;

            // Edit action
            var editButton = new CMSAccessibleButton
            {
                ToolTip = GetString("general.edit"),
                IconCssClass = "icon-edit",
                IconOnly = true
            };

            editButton.Click += (eventSender, args) => CategoryActionPerformed(eventSender, new CommandEventArgs("edit", catIdStr));
            rightPanel.Controls.Add(editButton);

            if (AllowEdit)
            {
                // Delete action
                var deleteButton = new CMSAccessibleButton
                {
                    ToolTip = GetString("general.delete"),
                    IconCssClass = "icon-bin",
                    IconOnly = true,
                    OnClientClick = string.Format("if (!confirm({0})) {{ return false; }}", ScriptHelper.GetString(GetString("Development.CustomSettings.GroupDeleteConfirmation")))
                };

                deleteButton.Click += (eventSender, args) => CategoryActionPerformed(eventSender, new CommandEventArgs("delete", catIdStr));
                rightPanel.Controls.Add(deleteButton);

                // Move up action
                var moveUpButton = new CMSAccessibleButton
                {
                    ToolTip = GetString("general.moveup"),
                    IconCssClass = "icon-chevron-up",
                    IconOnly = true
                };

                moveUpButton.Click += (eventSender, args) => CategoryActionPerformed(eventSender, new CommandEventArgs("moveup", catIdStr));
                rightPanel.Controls.Add(moveUpButton);

                // Move down action
                var moveDownButton = new CMSAccessibleButton
                {
                    ToolTip = GetString("general.movedown"),
                    IconCssClass = "icon-chevron-down",
                    IconOnly = true
                };

                moveDownButton.Click += (eventSender, args) => CategoryActionPerformed(eventSender, new CommandEventArgs("movedown", catIdStr));
                rightPanel.Controls.Add(moveDownButton);

                // Setup "Add key" button
                btnNewKey.Text = ResHelper.GetString("Development.CustomSettings.NewKey");
                btnNewKey.Click += CreateNewKey;

                cprModuleInfoRow.Visible = false;
                cprRow01.Visible = true;
            }
            else
            {
                ResourceInfo currentModule = ResourceInfo.Provider.Get(ModuleID);
                ResourceInfo categoryModule = ResourceInfo.Provider.Get(Category.CategoryResourceID);

                // Show warning if current module is in development mode, if not global warning is shown on the top of page
                if ((categoryModule != null) && (currentModule != null) && currentModule.ResourceIsInDevelopment)
                {
                    lblAnotherModule.Text = HTMLHelper.HTMLEncode(String.Format(GetString("settingscategory.disablededitting"), categoryModule.ResourceDisplayName));
                    cprModuleInfoRow.Visible = true;
                }
                else
                {
                    cprModuleInfoRow.Visible = false;
                }

                cprRow01.Visible = false;
            }

            gridElem.ShowObjectMenu = AllowEdit;

            // Panel title for group
            cpCategory.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(Category.CategoryDisplayName));

            // Filter out only records for this group
            gridElem.WhereCondition = "KeyCategoryID = " + Category.CategoryID;
        }

        // Apply site filter if required.
        if (!string.IsNullOrEmpty(gridElem.WhereCondition))
        {
            gridElem.WhereCondition += " AND ";
        }

        gridElem.WhereCondition += "SiteID IS NULL";
    }


    /// <summary>
    /// OnExternal databoud event handling (because of macro resolving).
    /// </summary>
    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "keydisplayname":
                return HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ValidationHelper.GetString(parameter, "")));

            case "ishidden":
                CMSGridActionButton ib = ((CMSGridActionButton)sender);
                ib.Visible = ValidationHelper.GetBoolean(((DataRowView)((GridViewRow)parameter).DataItem).Row["KeyIsHidden"], false);
                ib.OnClientClick = "javascript:return false;";
                break;

            case "action":
                // Set buttons visibility
                CMSGridActionButton actionButton = ((CMSGridActionButton)sender);
                if (!AllowEdit)
                {
                    actionButton.Enabled = false;
                }

                break;
        }

        return parameter;
    }
}