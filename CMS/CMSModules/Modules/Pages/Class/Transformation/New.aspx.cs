using System;

using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;

// Parent object
[ParentObject("cms.class", "classid")]

// Help topic
[Title(HelpTopic = "newedit_transformation")]

public partial class CMSModules_Modules_Pages_Class_Transformation_New : GlobalAdminPage
{
    #region "Properties"

    public int ClassID
    {
        get
        {
            return QueryHelper.GetInteger("classid", 0);
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        editElem.OnAfterSave += editElem_OnAfterSave;

        ucTransfCode.ClassID = ClassID;

        // Init breadcrumbs
        PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem()
        {
            Index = 0,
            Text = GetString("documenttype_edit_transformation_edit.transformations"),
            RedirectUrl = URLHelper.AppendQuery(UIContextHelper.GetElementUrl(ModuleName.CMS, "Transformations"), "parentobjectid=" + ClassID + "&classID=" + ClassID + "&displaytitle=false")
        });

        PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem()
        {
            Index = 1,
            Text = GetString("documenttype_edit_transformation_edit.newtransformation")
        });
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Generate default
        HeaderAction generate = new HeaderAction
        {
            Text = GetString("DocumentType_Edit_Transformation_Edit.ButtonDefault"),
            Tooltip = GetString("transformationtypecode.generatetooltip"),
            OnClientClick = "GenerateDefaultCode('default'); return false;"
        };

        HeaderActions.ActionsList.Add(generate);

        if (ucTransfCode.IsAscx)
        {
            generate.AlternativeActions.Add(new HeaderAction
            {
                Text = GetString("transformationtypecode.xml"),
                Tooltip = GetString("transformationtypecode.xmltooltip"),
                OnClientClick = "GenerateDefaultCode('xml'); return false;"
            });
        }

        base.OnPreRender(e);
    }


    protected void editElem_OnAfterSave(object sender, EventArgs e)
    {
        TransformationInfo ti = editElem.EditedObject as TransformationInfo;
        if (ti != null)
        {
            string editPage = URLHelper.AppendQuery(UIContextHelper.GetElementUrl(ModuleName.CMS, "EditTransformation"), "parentobjectid=" + ClassID + "&classID=" + ClassID + "&objectid=" + ti.TransformationID + "&displaytitle=false&saved=1");

            URLHelper.Redirect(editPage);
        }
    }
}