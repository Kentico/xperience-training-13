using System;

using CMS.DataEngine;
using CMS.Membership;
using CMS.OnlineForms;
using CMS.OnlineForms.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;


[EditedObject(BizFormInfo.OBJECT_TYPE, "formId")]
[Security(Resource = "CMS.Form", Permission = "ReadForm")]
[UIElement("cms.form", "Forms.SearchFields")]
public partial class CMSModules_BizForms_Tools_BizForm_Edit_SearchFields : CMSBizFormPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (EditedObject == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }

        BizFormInfo form = EditedObject as BizFormInfo;

        if (form == null)
        {
            return;
        }

        var dci = DataClassInfoProvider.GetDataClassInfo(form.FormClassID);

        // Class exists
        if ((dci == null) || (!dci.ClassIsForm))
        {
            ShowWarning(GetString("BizFormGeneral.NotAFormClass"));
            SearchFields.Visible = false;
            SearchFields.StopProcessing = true;
        }
        else
        {
            SearchFields.ItemID = dci.ClassID;
        }
            
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.form", "EditForm", SiteInfoProvider.GetSiteName(form.FormSiteID)))
        {
            RedirectToAccessDenied("cms.form", "EditForm");
        }
    }
}