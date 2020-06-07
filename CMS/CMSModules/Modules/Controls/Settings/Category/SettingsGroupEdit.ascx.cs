using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;


public partial class CMSModules_Modules_Controls_Settings_Category_SettingsGroupEdit : CMSUserControl
{
    #region "Properties"

    /// <summary>
    /// Code name of displayed category. This category is NOT group (CategoryIsGroup = false).
    /// </summary>
    public string CategoryName
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates the module identifier of current settings group.
    /// </summary>
    public int ModuleID
    {
        get;
        set;
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Event raised, when edit/delete/moveUp/moveDown category button is clicked.
    /// </summary>
    public event CommandEventHandler ActionPerformed;


    /// <summary>
    /// Event raised, when asked to add new key.
    /// </summary>
    public event CommandEventHandler OnNewKey;


    /// <summary>
    /// Event raised, when unigrid's button is clicked.
    /// </summary>
    public event OnActionEventHandler OnKeyAction;

    #endregion


    #region "Control events"

    /// <summary>
    /// Handles the whole category actions.
    /// </summary>
    /// <param name="sender">Sender of event</param>
    /// <param name="e">Event arguments</param>
    protected void group_ActionPerformed(object sender, CommandEventArgs e)
    {
        if (ActionPerformed != null)
        {
            ActionPerformed(sender, e);
        }
    }


    /// <summary>
    /// Handles request for creating of new settings key.
    /// </summary>
    /// <param name="sender">Sender of event</param>
    /// <param name="e">Event arguments</param>
    protected void group_OnNewKey(object sender, CommandEventArgs e)
    {
        if (OnNewKey != null)
        {
            OnNewKey(sender, e);
        }
    }


    /// <summary>
    /// Handles settings keys actions (delete, edit, move up, move down).
    /// </summary>
    /// <param name="actionName">Name of the action</param>
    /// <param name="actionArgument">Argument of the action</param>
    protected void group_OnKeyAction(string actionName, object actionArgument)
    {
        if (OnKeyAction != null)
        {
            OnKeyAction(actionName, actionArgument);
        }
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Reload data;
        ReloadData();
    }


    /// <summary>
    /// Creates SettingsGroup panel for a specified category.
    /// </summary>
    /// <param name="category">Non-group category</param>
    protected CMSUserControl CreatePanelForCategory(SettingsCategoryInfo category)
    {
        if (category == null)
        {
            return null;
        }

        // Create new Category bar and initialize it
        SettingsGroup group = Page.LoadUserControl("~/CMSModules/Modules/Controls/Settings/Category/SettingsGroup.ascx") as SettingsGroup;
        if (group != null)
        {
            group.Category = category;
            group.ModuleID = ModuleID;
            group.ActionPerformed += group_ActionPerformed;
            group.OnNewKey += group_OnNewKey;
            group.OnKeyAction += group_OnKeyAction;

            ResourceInfo resource = ResourceInfo.Provider.Get(ModuleID);

            group.AllowEdit = (resource != null) && ((resource.ResourceIsInDevelopment && (resource.ResourceID == category.CategoryResourceID)) || SystemContext.DevelopmentMode);        
        }

        return group;
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    public void ReloadData()
    {
        // Get data
        DataSet ds = SettingsCategoryInfoProvider.GetChildSettingsCategories(CategoryName, "CategoryIsGroup = 1");

        Controls.Clear();

        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            DataRowCollection rows = ds.Tables[0].Rows;
            foreach (DataRow row in rows)
            {
                // Create new panel with info about subcategory
                var sci = new SettingsCategoryInfo(row);
                CMSUserControl catPanel = CreatePanelForCategory(sci);

                catPanel.ID = ControlsHelper.GetUniqueID(this, "CategoryPanel_" + ValidationHelper.GetIdentifier(sci.CategoryName));
                Controls.Add(catPanel);
            }
        }
    }

    #endregion
}