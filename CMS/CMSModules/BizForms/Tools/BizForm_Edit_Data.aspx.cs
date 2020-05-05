using System;
using System.Linq;

using CMS.OnlineForms;
using CMS.OnlineForms.Web.UI;
using CMS.UIControls;

// Edited object
[EditedObject(BizFormInfo.OBJECT_TYPE, "formId")]
[Security(Resource = "CMS.Form", Permission= "ReadData")]
[UIElement("CMS.Form", "Forms.Data")]
public partial class CMSModules_BizForms_Tools_BizForm_Edit_Data : CMSBizFormPage
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
}
