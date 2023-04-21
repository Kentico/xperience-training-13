using System;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_Settings_Pages_Categories : CMSDeskPage, ICallbackEventHandler
{
    // key is connected to default page on purpose, where the value is used for frame dimensions
    private const string UI_LAYOUT_KEY = nameof(CMSModules_Settings_Pages_Default);


    /// <summary>
    /// OnPreLoad event. 
    /// </summary>
    protected override void OnPreLoad(EventArgs e)
    {
        base.OnPreLoad(e);
        RequireSite = false;
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!RequestHelper.IsPostBack() && !RequestHelper.IsCallback())
        {
            var width = UILayoutHelper.GetLayoutWidth(UI_LAYOUT_KEY);
            if (width.HasValue)
            {
                TreeViewCategories.TreePane.Size = width.ToString();
            }
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        TreeViewCategories.Layout.OnResizeEndScript = ScriptHelper.GetLayoutResizeScript(TreeViewCategories.TreePane, this);
        TreeViewCategories.Layout.MaxSize = "50%";

        ScriptHelper.RegisterJQuery(Page);

        TreeViewCategories.ShowHeaderPanel = CMSActionContext.CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin);
        TreeViewCategories.RootIsClickable = true;

        // Get selected category ID
        int categoryId;
        if (!RequestHelper.IsPostBack() && QueryHelper.Contains("selectedCategoryId"))
        {
            // Get from URL
            categoryId = QueryHelper.GetInteger("selectedCategoryId", 0);
        }
        else if (Request.Form["selectedCategoryId"] != null)
        {
            // Get from postback
            categoryId = ValidationHelper.GetInteger(Request.Form["selectedCategoryId"], 0);
        }
        else
        {
            // Select root by default
            categoryId = SettingsCategoryInfoProvider.GetRootSettingsCategoryInfo().CategoryID;
        }
        TreeViewCategories.CategoryID = categoryId;
    }


    /// <summary>
    /// Reloads tree content.
    /// </summary>
    protected override void OnLoadComplete(EventArgs e)
    {
        base.OnLoadComplete(e);

        // Reload tree after selected site has changed.
        if (RequestHelper.IsPostBack())
        {
            TreeViewCategories.ReloadData();
        }
    }


    void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
    {
        var parsed = eventArgument.Split(new[] { UILayoutHelper.DELIMITER });
        if (parsed.Length == 2 && String.Equals(UILayoutHelper.WIDTH_ARGUMENT, parsed[0], StringComparison.OrdinalIgnoreCase))
        {
            if (int.TryParse(parsed[1], out var width))
            {
                UILayoutHelper.SetLayoutWidth(UI_LAYOUT_KEY, width);
            }
        }
    }


    string ICallbackEventHandler.GetCallbackResult()
    {
        return null;
    }
}
