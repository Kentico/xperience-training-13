using System;
using System.Web;
using System.Collections.Specialized;

using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;


[UIElement(ModuleName.CMS, "Modules.UserInterface.New")]
public partial class CMSModules_Modules_Pages_Module_UserInterface_New : GlobalAdminPage
{
    private int? mParentID;


    private int ParentID
    {
        get
        {
            return mParentID ?? (mParentID = QueryHelper.GetInteger("parentId", 0)).Value;
        }
    }


    protected void Page_Init(object sender, EventArgs e)
    {
        editElem.Form.OnAfterDataLoad += Form_OnAfterDataLoad;
        editElem.Form.OnBeforeSave += Form_OnBeforeSave;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle.TitleText = GetString("resource.ui.title");

        if (ParentID > 0)
        {
            int moduleId = QueryHelper.GetInteger("moduleid", 0);
            editElem.ResourceID = moduleId;
            editElem.ParentID = ParentID;

            // Init breadcrumbs
            PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem()
            {
                Text = GetString("resource.ui.element"),
                RedirectUrl = GetEditRedirectURL(moduleId)
            });

            PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem()
            {
                Text = GetString("resource.ui.newelement"),
            });
        }
        else
        {
            editElem.Visible = false;
            lblInfo.Visible = true;
            lblInfo.Text = GetString("resource.ui.rootelement");
        }
    }


    /// <summary>
    /// Set default value of DisplayBreadcrumbs checkbox depending on element level.
    /// </summary>
    private void Form_OnAfterDataLoad(object sender, EventArgs e)
    {
        var parent = UIElementInfo.Provider.Get(ParentID);

        // Show breadcrumbs checkbox depending on element level         
        bool displayBreadcrumbs = (parent != null) && (parent.ElementLevel >= UIElementInfoProvider.APPLICATION_LEVEL);
        if (!displayBreadcrumbs)
        {
            editElem.Form.FieldsToHide.Add("ElementDisplayBreadcrumbs");
        }
    }


    /// <summary>
    /// Save value of DisplayBreadcrumbs checkbox into <see cref="UIElementInfo.ElementProperties"/> of the <see cref="UIElementInfo"/>.
    /// </summary>
    private void Form_OnBeforeSave(object sender, EventArgs e)
    {
        var elementInfo = (UIElementInfo)editElem.Form.EditedObject;

        // Create XMLData collection for current element
        XmlData data = new XmlData();
        data.LoadData(elementInfo.ElementProperties);

        // Set property DisplayBreadcrumbs to value of checkbox
        data["DisplayBreadcrumbs"] = editElem.Form.FieldControls["ElementDisplayBreadcrumbs"].Value;
        elementInfo.ElementProperties = data.GetData();
    }


    /// <summary>
    /// Builds redirect URL with query to edit page.
    /// </summary>
    private string GetEditRedirectURL(int moduleId)
    {
        NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);

        query["elementId"] = ParentID.ToString();
        query["moduleid"] = moduleId.ToString();
        query["objectParentId"] = query["moduleid"];

        string url = UIContextHelper.GetElementUrl(ModuleName.CMS, "Modules.UserInterface.Edit", false, ParentID);
        string urlWithQuery = URLHelper.AppendQuery(url, query.ToString());

        return urlWithQuery;
    }
}