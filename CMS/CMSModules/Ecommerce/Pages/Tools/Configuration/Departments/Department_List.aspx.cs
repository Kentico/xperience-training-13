using System;

using CMS.Base;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.Ecommerce.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[Title("Department_List.HeaderCaption")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_Departments_Department_List : CMSDepartmentsPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Init Unigrid
        UniGrid.OnAction += uniGrid_OnAction;
        UniGrid.WhereCondition = InitSiteWhereCondition("DepartmentSiteID").ToString(true);
        HandleGridsSiteIDColumn(UniGrid);
        UniGrid.RememberStateByParam = GetGridRememberStateParam();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        string newElementName;

        if (IsMultiStoreConfiguration)
        {
            newElementName = "Ecommerce.GlobalDepartments.New";
            CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, "Ecommerce.GlobalDepartments");
        }
        else
        {
            newElementName = "new.configuration.Departments";
            CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, "Configuration.Departments");
        }

        // Header actions
        var actions = CurrentMaster.HeaderActions;
        actions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("Department_List.newitemcaption"),
            RedirectUrl = GetRedirectURL(newElementName),
        });
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            var editElementName = IsMultiStoreConfiguration ? "Edit.Ecommerce.GlobalDepartments.General" : "Edit.Configuration.Departments.General";
            URLHelper.Redirect(UIContextHelper.GetElementUrl(ModuleName.ECOMMERCE, editElementName, false, actionArgument.ToInteger(0)));
        }
    }


    /// <summary>
    /// Generates redirection url with query string parameters.
    /// </summary>
    /// <param name="uiElementName">Name of ui element to redirect to.</param>
    private string GetRedirectURL(string uiElementName)
    {
        var url = UIContextHelper.GetElementUrl(ModuleName.ECOMMERCE, uiElementName, false);
        // Only global object can be created from site manager       
        if (IsMultiStoreConfiguration)
        {
            url = URLHelper.AddParameterToUrl(url, "siteid", SpecialFieldValue.GLOBAL.ToString());
        }

        return url;
    }
}