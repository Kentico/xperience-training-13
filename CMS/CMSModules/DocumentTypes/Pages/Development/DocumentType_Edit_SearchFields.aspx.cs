using System;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_SearchFields : GlobalAdminPage
{
    private int classId = QueryHelper.GetInteger("objectid", 0);


    protected void Page_Load(object sender, EventArgs e)
    {
        DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(classId);
        if ((dci == null) || (!dci.ClassHasURL && !dci.ClassIsCoupledClass))
        {
            // Show error message
            ShowError(GetString("srch.doctype.ErrorCannotHaveSearchFieldsOptions"));

            SearchFields.StopProcessing = true;
            SearchFields.Visible = false;
        }
        else
        {
            SearchFields.ItemID = classId;
        }
    }
}